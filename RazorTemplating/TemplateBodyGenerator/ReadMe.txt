1. Add template views to "Templates" folder.
2. Cshtml files must be all lower case.
3. Partial template names must start with "_"
4. Set properties for each cshtml file to:
    Build Action: Embedded Resource
    Copy To Output Directory:  Do Not Copy

5. Add a Model implementing IEmailTemplateModel for each template file
