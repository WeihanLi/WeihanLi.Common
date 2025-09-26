#!/usr/bin/env python3

import os
import re
import subprocess

def run_build():
    """Run dotnet build and capture output"""
    result = subprocess.run(['dotnet', 'build'], 
                          capture_output=True, text=True, cwd='/home/runner/work/WeihanLi.Common/WeihanLi.Common')
    with open('build_output.log', 'w') as f:
        f.write(result.stdout)
        f.write(result.stderr)
    return result.returncode == 0

def get_errors():
    """Parse build errors from log"""
    errors = []
    with open('build_output.log', 'r') as f:
        for line in f:
            if 'error IL' in line:
                errors.append(line.strip())
    return errors

def fix_method_aot_attributes():
    """Add missing AOT attributes to methods"""
    
    # Methods that need RequiresDynamicCode attribute
    methods_need_dynamic = [
        ('src/WeihanLi.Common/Extensions/DataExtension.cs', 'ExecuteDataTable<T>'),
        ('src/WeihanLi.Common/Extensions/DataExtension.cs', 'ExecuteDataTableAsync<T>'),
        ('src/WeihanLi.Common/Extensions/DataExtension.cs', 'ToEntity<T>'),
        ('src/WeihanLi.Common/Extensions/DataExtension.cs', 'ToEntities<T>'),
        ('src/WeihanLi.Common/Extensions/DataExtension.cs', 'ToDataTable<T>'),
    ]
    
    # Add attributes to specific methods
    for file_path, method_name in methods_need_dynamic:
        if os.path.exists(file_path):
            print(f"Adding AOT attributes to {method_name} in {file_path}")
            add_aot_attributes_to_method(file_path, method_name)

def add_aot_attributes_to_method(file_path, method_name):
    """Add AOT attributes to a specific method"""
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Find method and add attributes if not present
    method_pattern = rf'([ \t]*)(public[^{{]*{re.escape(method_name)}[^{{]*\{{)'
    
    def replace_method(match):
        indent = match.group(1)
        method_def = match.group(2)
        
        # Check if attributes already exist
        preceding_lines = content[:match.start()].split('\n')[-10:]  # Look at last 10 lines
        has_requires_dynamic = any('[RequiresDynamicCode' in line for line in preceding_lines)
        has_requires_unreferenced = any('[RequiresUnreferencedCode' in line for line in preceding_lines)
        
        attributes = []
        if not has_requires_dynamic:
            attributes.append(f'{indent}[RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]')
        if not has_requires_unreferenced:
            attributes.append(f'{indent}[RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can\'t validate that the requirements of those annotations are met.")]')
        
        if attributes:
            return '\n'.join(attributes) + '\n' + indent + method_def
        return match.group(0)
    
    new_content = re.sub(method_pattern, replace_method, content, flags=re.MULTILINE)
    
    if new_content != content:
        with open(file_path, 'w') as f:
            f.write(new_content)
        print(f"Updated {file_path}")

def main():
    os.chdir('/home/runner/work/WeihanLi.Common/WeihanLi.Common')
    
    print("Starting comprehensive AOT fix...")
    
    # Run initial build to get current errors
    print("Running initial build...")
    run_build()
    
    initial_errors = get_errors()
    print(f"Initial errors: {len(initial_errors)}")
    
    # Fix method attributes
    fix_method_aot_attributes()
    
    # Run build again to check progress
    print("Running build after fixes...")
    run_build()
    
    final_errors = get_errors()
    print(f"Final errors: {len(final_errors)}")
    print(f"Reduced by: {len(initial_errors) - len(final_errors)} errors")

if __name__ == "__main__":
    main()