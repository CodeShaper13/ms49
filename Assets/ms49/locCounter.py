import os
import fnmatch

def Walk(root='.', recurse=True, pattern='*'):
    """
        Generator for walking a directory tree.
        Starts at specified root folder, returning files
        that match our pattern. Optionally will also
        recurse through sub-folders.
    """
    for path, subdirs, files in os.walk(root):
        for name in files:
            if fnmatch.fnmatch(name, pattern):
                yield os.path.join(path, name)
        if not recurse:
            break

def LOC(root='.', recurse=True):
    """
        Counts lines of code in two ways:
            maximal size (source LOC) with blank lines and comments
            minimal size (logical LOC) stripping same

        Sums all Python files in the specified folder.
        By default recurses through subfolders.
    """
    count_mini, count_maxi = 0, 0
    for fspec in Walk(root, recurse, '*.cs'):
        i = 0
        skip = False
        for line in open(fspec).readlines():
            count_maxi += 1
            i += 1
            line = line.strip()
            if line:
                if line.startswith('#'):
                    continue
                if line.startswith('"""'):
                    skip = not skip
                    continue
                if not skip:
                    count_mini += 1
        print(str(i) + "\t" + fspec)

    return count_mini, count_maxi

lines = LOC()
print "LOC: \n\tReal Lines: " + str(lines[0]) + "\n\tTotal Lines: " + str(lines[1])

print "\nPress ENTER to close"
input()
