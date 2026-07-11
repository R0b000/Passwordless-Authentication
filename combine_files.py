import os
import sys
from pathlib import Path
from typing import List, Set

def combine_code_files(root_folder: str, ignore_folders: Set[str] = None, output_file: str = None):
    """
    Combine all code files with folder structure.
    
    Args:
        root_folder: Path to the root folder to scan
        ignore_folders: Set of folder names to ignore (default: bin, obj, node_modules, etc.)
        output_file: Optional output file path. If None, prints to console
    """
    if ignore_folders is None:
        ignore_folders = {'bin', 'obj', 'node_modules', '.git', '.vs', '.vscode', 'packages', 'wwwroot'}
    
    # Common code file extensions to include
    code_extensions = {
        '.cs', '.cshtml', '.razor', '.css', '.js', '.ts', '.json', '.xml', 
        '.config', '.csproj', '.sln', '.html', '.scss', '.sass', '.less',
        '.py', '.java', '.cpp', '.h', '.hpp', '.sql', '.md', '.txt', '.yml', '.yaml'
    }
    
    if output_file:
        output = open(output_file, 'w', encoding='utf-8')
    else:
        output = sys.stdout
    
    try:
        root_path = Path(root_folder).resolve()
        
        # Write header
        output.write("=" * 80 + "\n")
        output.write(f"COMBINED CODE FILES FROM: {root_path}\n")
        output.write("=" * 80 + "\n\n")
        
        # Collect all files first
        all_files = []
        folder_structure = []
        
        for dirpath, dirnames, filenames in os.walk(root_path):
            # Remove ignored folders from dirnames to prevent walking into them
            dirnames[:] = [d for d in dirnames if d not in ignore_folders]
            
            # Calculate relative path
            rel_dir = Path(dirpath).relative_to(root_path)
            
            # Add to folder structure
            if str(rel_dir) != '.':
                indent = "  " * (len(rel_dir.parts) - 1)
                folder_structure.append(f"{indent}📁 {rel_dir.name}/")
            
            # Add files
            for filename in sorted(filenames):
                file_path = Path(dirpath) / filename
                
                # Check if it's a code file
                if file_path.suffix.lower() in code_extensions or filename.startswith('.'):
                    rel_path = file_path.relative_to(root_path)
                    indent = "  " * len(rel_path.parts[:-1])
                    folder_structure.append(f"{indent}📄 {filename}")
                    all_files.append(file_path)
        
        # Write folder structure
        output.write("FOLDER STRUCTURE:\n")
        output.write("-" * 80 + "\n")
        output.write(f" {root_path.name}/\n")
        for line in folder_structure:
            output.write(f"{line}\n")
        output.write("\n")
        
        # Write file contents
        output.write("\n" + "=" * 80 + "\n")
        output.write("FILE CONTENTS:\n")
        output.write("=" * 80 + "\n\n")
        
        for file_path in all_files:
            rel_path = file_path.relative_to(root_path)
            
            output.write("-" * 80 + "\n")
            output.write(f"FILE: {rel_path}\n")
            output.write("-" * 80 + "\n")
            
            try:
                with open(file_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    output.write(content)
                    if not content.endswith('\n'):
                        output.write('\n')
            except Exception as e:
                output.write(f"[ERROR reading file: {e}]\n")
            
            output.write("\n\n")
        
        output.write("=" * 80 + "\n")
        output.write("END OF COMBINED FILES\n")
        output.write("=" * 80 + "\n")
        
    finally:
        if output_file and output:
            output.close()

def main():
    """Main function with command line interface"""
    if len(sys.argv) < 2:
        print("Usage:")
        print("  python combine_files.py <folder_path> [output_file.txt]")
        print("  python combine_files.py <folder_path> --ignore folder1,folder2 [output_file.txt]")
        print("\nExamples:")
        print("  python combine_files.py ./PasswordlessApi")
        print("  python combine_files.py ./PasswordlessApi combined_code.txt")
        print("  python combine_files.py ./PasswordlessApi --ignore bin,obj,wwwroot combined_code.txt")
        sys.exit(1)
    
    # Parse arguments
    args = sys.argv[1:]
    root_folder = args[0]
    output_file = None
    ignore_folders = None
    
    i = 1
    while i < len(args):
        if args[i] == '--ignore' and i + 1 < len(args):
            ignore_folders = set(args[i + 1].split(','))
            i += 2
        elif not args[i].startswith('--'):
            output_file = args[i]
            i += 1
        else:
            i += 1
    
    # Validate folder
    if not os.path.isdir(root_folder):
        print(f"Error: '{root_folder}' is not a valid directory")
        sys.exit(1)
    
    print(f"Combining files from: {root_folder}")
    if ignore_folders:
        print(f"Ignoring folders: {', '.join(ignore_folders)}")
    if output_file:
        print(f"Output file: {output_file}")
    print("-" * 80)
    
    combine_code_files(root_folder, ignore_folders, output_file)
    
    if output_file:
        print(f"\n✓ Files combined successfully into: {output_file}")
    else:
        print("\n✓ Done!")

if __name__ == "__main__":
    main()