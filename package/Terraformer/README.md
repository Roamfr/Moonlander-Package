# UnityPackageTemplate
This is a bootstrap template to create repositories for Unity Packages

# Instructions
* Create a new repository from this template.
* Clone the new repository into an existing or new Unity Project (inside Assets folder).
* Edit the package.json file and replace all the fields in [], add dependencies as needed.
* Edit the assembly references on Runtime and Editor folders, change the name "UnityPackageTemplate" to the name of the package and check the other settings (references to other dependency packages might be needed).
* Add runtime scripts/assets in the Runtime folder (Use Moonlander.[PackageName] as namespace).
* Add editor scripts and files in the Editor folder (Use Moonlander.[PackageName] as namespace).
 