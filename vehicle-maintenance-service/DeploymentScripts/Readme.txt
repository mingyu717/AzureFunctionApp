This folder contains Octopus Deployment Scripts for all the environments we deploy to.
When this solution is built and packaged these folders will be present in the Octopus package so it can reference the deploy scripts from inside the package.
If we want to make changes to Deployment scripts change them in source then commit and merge to master so Octopus gets an updated copy.