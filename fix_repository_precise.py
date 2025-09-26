import re

# Read the file
with open('src/WeihanLi.Common/Data/Repository.cs', 'r') as f:
    content = f.read()

# Find all public virtual methods that don't already have AOT attributes
lines = content.split('\n')
result_lines = []
i = 0

while i < len(lines):
    line = lines[i]
    
    # Check if this line is a public virtual method
    if re.match(r'^\s+public virtual ', line) and not line.strip().startswith('//'):
        # Check if the previous lines already have AOT attributes
        has_requires_dynamic = False
        has_requires_unreferenced = False
        
        # Look at the previous few lines to see if attributes exist
        for j in range(max(0, i-5), i):
            prev_line = lines[j].strip()
            if '[RequiresDynamicCode' in prev_line:
                has_requires_dynamic = True
            if '[RequiresUnreferencedCode' in prev_line:
                has_requires_unreferenced = True
        
        # Add attributes if they don't exist
        indent = line[:len(line) - len(line.lstrip())]  # Get the indentation
        
        if not has_requires_dynamic:
            result_lines.append(f'{indent}[RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]')
        if not has_requires_unreferenced:
            result_lines.append(f'{indent}[RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]')
    
    result_lines.append(line)
    i += 1

# Write back to file
with open('src/WeihanLi.Common/Data/Repository.cs', 'w') as f:
    f.write('\n'.join(result_lines))

print("AOT attributes added precisely to Repository methods")
