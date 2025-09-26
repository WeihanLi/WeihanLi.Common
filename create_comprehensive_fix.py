#!/usr/bin/env python3

import os
import re

def fix_repository_extension():
    """Fix RepositoryExtension.cs methods"""
    file_path = 'src/WeihanLi.Common/Data/RepositoryExtension.cs'
    
    if not os.path.exists(file_path):
        return
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Add using statement if not present
    if 'using System.Diagnostics.CodeAnalysis;' not in content:
        content = content.replace('using System.Linq.Expressions;\n', 
                                'using System.Linq.Expressions;\nusing System.Diagnostics.CodeAnalysis;\n')
    
    # Add AOT attributes to all public static methods
    method_pattern = r'(\s*)(public static [^{]*\{)'
    
    def add_attributes(match):
        indent = match.group(1)
        method_def = match.group(2)
        
        # Check if attributes already exist in preceding lines
        preceding_lines = content[:match.start()].split('\n')[-5:]
        has_dynamic = any('[RequiresDynamicCode' in line for line in preceding_lines)
        has_unreferenced = any('[RequiresUnreferencedCode' in line for line in preceding_lines)
        
        if not has_dynamic and not has_unreferenced:
            attributes = [
                f'{indent}[RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]',
                f'{indent}[RequiresUnreferencedCode("Database operations may use reflection which can break functionality when trimming application code.")]'
            ]
            return '\n'.join(attributes) + '\n' + indent + method_def
        
        return match.group(0)
    
    new_content = re.sub(method_pattern, add_attributes, content, flags=re.MULTILINE)
    
    if new_content != content:
        with open(file_path, 'w') as f:
            f.write(new_content)
        print(f"Fixed {file_path}")

def fix_extension_methods():
    """Fix extension methods that call AOT-annotated methods"""
    
    files_to_fix = [
        'src/WeihanLi.Common/Extensions/HttpRequesterExtension.cs',
        'src/WeihanLi.Common/Extensions/ServiceCollectionExtension.cs',
        'src/WeihanLi.Common/Extensions/ExpressionExtension.cs',
    ]
    
    for file_path in files_to_fix:
        if os.path.exists(file_path):
            fix_extension_file(file_path)

def fix_extension_file(file_path):
    """Add AOT attributes to extension methods in a file"""
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Add using statement if not present
    if 'using System.Diagnostics.CodeAnalysis;' not in content:
        lines = content.split('\n')
        # Find last using statement and add after it
        last_using_idx = -1
        for i, line in enumerate(lines):
            if line.strip().startswith('using ') and not line.strip().startswith('using static'):
                last_using_idx = i
        
        if last_using_idx != -1:
            lines.insert(last_using_idx + 1, 'using System.Diagnostics.CodeAnalysis;')
            content = '\n'.join(lines)
    
    # Add AOT attributes to public static methods that don't have them
    method_pattern = r'(\s*)(public static [^{]*(?:Async|<T>|Expression)[^{]*\{)'
    
    def add_attributes(match):
        indent = match.group(1)
        method_def = match.group(2)
        
        # Check if attributes already exist
        preceding_lines = content[:match.start()].split('\n')[-5:]
        has_dynamic = any('[RequiresDynamicCode' in line for line in preceding_lines)
        has_unreferenced = any('[RequiresUnreferencedCode' in line for line in preceding_lines)
        
        if not has_dynamic and not has_unreferenced:
            attributes = [
                f'{indent}[RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]',
                f'{indent}[RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can\'t validate that the requirements of those annotations are met.")]'
            ]
            return '\n'.join(attributes) + '\n' + indent + method_def
        
        return match.group(0)
    
    new_content = re.sub(method_pattern, add_attributes, content, flags=re.MULTILINE)
    
    if new_content != content:
        with open(file_path, 'w') as f:
            f.write(new_content)
        print(f"Fixed {file_path}")

def fix_event_bus_extension():
    """Fix EventBusExtensions methods"""
    file_path = 'src/WeihanLi.Common/Event/EventBusExtensions.cs'
    
    if not os.path.exists(file_path):
        return
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Add RequiresDynamicCode and RequiresUnreferencedCode to GetEventHandlers method
    if '[RequiresDynamicCode' not in content:
        # Find the GetEventHandlers method
        pattern = r'(\s*)(public static [^{]*GetEventHandlers[^{]*\{)'
        
        def add_attributes(match):
            indent = match.group(1)
            method_def = match.group(2)
            
            attributes = [
                f'{indent}[RequiresDynamicCode("Event handler retrieval may require dynamic code generation.")]',
                f'{indent}[RequiresUnreferencedCode("Event handler types may be trimmed if not referenced elsewhere.")]'
            ]
            return '\n'.join(attributes) + '\n' + indent + method_def
        
        new_content = re.sub(pattern, add_attributes, content, flags=re.MULTILINE)
        
        if new_content != content:
            with open(file_path, 'w') as f:
                f.write(new_content)
            print(f"Fixed {file_path}")

def fix_service_collection_extensions():
    """Fix ServiceCollectionExtension specific methods"""
    file_path = 'src/WeihanLi.Common/Extensions/ServiceCollectionExtension.cs'
    
    if not os.path.exists(file_path):
        return
    
    with open(file_path, 'r') as f:
        content = f.read()
    
    # Add using statement if not present
    if 'using System.Diagnostics.CodeAnalysis;' not in content:
        content = content.replace('using Microsoft.Extensions.DependencyInjection;\n', 
                                'using Microsoft.Extensions.DependencyInjection;\nusing System.Diagnostics.CodeAnalysis;\n')
    
    # Add AOT attributes to methods that use generic types or reflection
    methods_need_attributes = [
        'TryAddEnumerable',
        'TryAddImplementations',
        'RegisterTypeAsImplementedInterfaces',
        'RegisterAssemblyTypes',
        'AddDelegateService'
    ]
    
    for method_name in methods_need_attributes:
        pattern = rf'(\s*)(public static [^{{]*{method_name}[^{{]*\{{)'
        
        def add_attributes(match):
            indent = match.group(1)
            method_def = match.group(2)
            
            # Check if attributes already exist
            preceding_lines = content[:match.start()].split('\n')[-5:]
            has_dynamic = any('[RequiresDynamicCode' in line for line in preceding_lines)
            has_unreferenced = any('[RequiresUnreferencedCode' in line for line in preceding_lines)
            
            if not has_dynamic and not has_unreferenced:
                attributes = [
                    f'{indent}[RequiresDynamicCode("Service registration may require dynamic code generation.")]',
                    f'{indent}[RequiresUnreferencedCode("Service types may be trimmed if not referenced elsewhere.")]'
                ]
                return '\n'.join(attributes) + '\n' + indent + method_def
            
            return match.group(0)
        
        content = re.sub(pattern, add_attributes, content, flags=re.MULTILINE)
    
    with open(file_path, 'w') as f:
        f.write(content)
    print(f"Fixed {file_path}")

def main():
    os.chdir('/home/runner/work/WeihanLi.Common/WeihanLi.Common')
    
    print("Starting comprehensive AOT fixes...")
    
    # Fix specific problem files
    fix_repository_extension()
    fix_extension_methods()
    fix_event_bus_extension()
    fix_service_collection_extensions()
    
    print("AOT fixes completed!")

if __name__ == "__main__":
    main()