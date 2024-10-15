![](banner.png)

# Contributing

## Pull Requests

### Setting Up A Local Environment

For editors of the documentation, it is necessary to set up a local environment where you can edit and preview changes before pushing them to the repository. The following lists the basics steps to do this.

**Step 1:** Navigate to our [Github Repository](https://github.com/GoInterject/ids-dotnet-api){:target="\_blank"}{:rel="noopener noreferrer"}.

Click the clone button and then the copy to clipboard icon to copy the repository's URL:

```
https://github.com/GoInterject/ids-dotnet-api.git
```

**Step 2:** Navigate to a local folder where you want to clone the repository.

Execute the git clone command (be sure to paste the url you copied):

```bash
git clone https://github.com/GoInterject/ids-dotnet-api.git
```

**Step 3:** Make your changes locally to a local git branch.

**Step 4:** Push your changes to the repo using the git push command.

```bash
git push origin local_branch:remote_branch
```

**Step 5:** Make a pull request to merge the remote_branch you pushed the changes to into the develop branch.

Click **Pull requests** in the header and then **New pull request**.

![](/docs/source/static/new-pull-request.png)
<br>

Ensure you are requesting to merge your branch into the develop branch. Click **Create pull request**.

![](/docs/source/static/init-pull-request.png)
<br>

Write a detailed description and then click the Reviewers settings button to choose a reviewer. Finally click **Create pull request**.

![](/docs/source/static/create-pull-request.png)
<br>

### Commit Messages

All commit messages should follow this general layout. This is to ensure consistency in determining what has been changed.

```
action(thingchanged): description of the change
```

| Actions | When to Use                                 |
| ------- | ------------------------------------------- |
| update  | Modifying existing code/refactoring         |
| add     | Adding new material, objects, classes, etc. |
| remove  | Removing content                            |

The "thingchanged" should be a short phrase that best describes the piece of code that was targeted.

The "description of the change" should be an accurate description of the changes.

Example:

```
add(IDSColumn object): added parameter 'is_nullable'
```

## Building the Doc Site

The doc site can be built with docfx:

```bash
cd ids-dotnet-api/docs
docfx build
```

To serve the site, run:

```bash
cd ids-dotnet-api/docs
docfx serve site
```

The html docs are then located in the `docs/site` folder. To open the doc site:

```ps1
# windows
start "./docs/site/index.html"

# mac \ linux
open "./docs/site/index.html"
```
