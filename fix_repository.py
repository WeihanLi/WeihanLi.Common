import re

# Read the file
with open('src/WeihanLi.Common/Data/Repository.cs', 'r') as f:
    content = f.read()

# Pattern to match public virtual methods
pattern = r'(\n    )(public virtual [^{]+)'

# Replacement function
def add_attributes(match):
    indent = match.group(1)
    method_signature = match.group(2)
    
    # Add the AOT attributes before the method
    attributes = f'{indent}[RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]\n'
    attributes += f'{indent}[RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]\n'
    attributes += f'{indent}{method_signature}'
    
    return attributes

# Apply the replacement
modified_content = re.sub(pattern, add_attributes, content)

# Write back to file
with open('src/WeihanLi.Common/Data/Repository.cs', 'w') as f:
    f.write(modified_content)

print("AOT attributes added to Repository methods")
