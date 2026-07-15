import os
from pathlib import Path

def combine_all_directories(directories, output_file):
    """Combine multiple directories into one single output file"""
    
    # Common file extensions to include
    extensions = {'.cs', '.js', '.html', '.css', '.ts', '.json', '.xml', '.sql', '.txt'}
    
    output_path = Path(output_file)
    total_files = 0
    
    try:
        with open(output_path, 'w', encoding='utf-8') as outfile:
            # Write header
            outfile.write("# Combined Project Files\n")
            outfile.write(f"# Generated: {__import__('datetime').datetime.now()}\n")
            outfile.write("=" * 80 + "\n\n")
            
            for source_dir in directories:
                source_path = Path(source_dir)
                
                if not source_path.exists():
                    print(f"❌ Warning: Directory '{source_dir}' not found, skipping...")
                    continue
                
                outfile.write(f"\n\n{'=' * 80}\n")
                outfile.write(f"# DIRECTORY: {source_dir}\n")
                outfile.write(f"{'=' * 80}\n\n")
                
                print(f"\nProcessing: {source_dir}")
                files_added = 0
                
                for root, dirs, files in os.walk(source_path):
                    # Skip common directories
                    dirs[:] = [d for d in dirs if d not in {'.git', 'node_modules', 'bin', 'obj', 'Properties'}]
                    
                    for file in files:
                        file_path = Path(root) / file
                        if file_path.suffix.lower() in extensions:
                            try:
                                rel_path = file_path.relative_to(source_path)
                                outfile.write(f"\n{'#' * 80}\n")
                                outfile.write(f"# {source_dir}/{rel_path}\n")
                                outfile.write(f"{'#' * 80}\n\n")
                                
                                with open(file_path, 'r', encoding='utf-8') as infile:
                                    content = infile.read()
                                    outfile.write(content)
                                    if not content.endswith('\n'):
                                        outfile.write('\n')
                                
                                files_added += 1
                                print(f"  ✓ Added: {rel_path}")
                            except Exception as e:
                                print(f"  ⚠ Skipped: {file_path.name} ({e})")
                
                total_files += files_added
                print(f"  → Added {files_added} files from {source_dir}")
        
        print(f"\n{'=' * 80}")
        print(f"✅ Successfully combined {total_files} files into: {output_file}")
        print(f"{'=' * 80}")
        return True
        
    except Exception as e:
        print(f"❌ Error: {e}")
        return False

def main():
    directories = [
        r".\PasswordlessApi",
        r".\Auth.UI",
        r".\Database"
    ]
    
    output_file = "Combined_Project.txt"
    
    combine_all_directories(directories, output_file)

if __name__ == "__main__":
    main()