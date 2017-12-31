# Adding and Updating Docs
So you've discovered some new stuff in Forge Networking and you think that it would be important to add documentation around your discovery to share with the community. Great!! We love that and people like you is where real progress comes from :). So lets begin with the requirements to get started:

1. Access to GitHub (You can get this by requesting it in Discord and providing your GitHub username)
2. An updated checkout of your own fork of the [Forge Networking Remastered](https://github.com/BeardedManStudios/ForgeNetworkingRemastered) repository
3. A text editor to edit Markdown (.md) files in, we recommend [Visual Studio Code](https://code.visualstudio.com/)

And that is it! You are officially ready to start adding and updating the documentation via pull requests.

## Creating a New Page in the Docs
We use MkDocs to generate our searchable documentation so it is extremely easy for you to create your page using Markdown. Below are the steps taken to create your own page:

1. Open the `ForgeNetworkingRemastered/docs/mkdocs/docs` folder in the repo
2. Create your file, if your page is called **Azure Setup** then you will create a file **azure-setup.md** in this folder
3. Open and edit your .md file
4. Save your file
5. Open the `ForgeNetworkingRemastered/docs/mkdocs/mkdocs.yml` file
6. Add your file to the appropriate section under **pages:** or create a new section if needed
7. Commit your changes and create a pull request

Once your pull request is accepted, we will run the MkDocs build and your documentation will be up on the website!

## Updating an Existing Page in the Docs
If you find an error or feel that something is missing from the documentation you can do make a fix and submit it for review! Below are the steps in updating a page in the documentation:

1. Open the `ForgeNetworkingRemastered/docs/mkdocs/docs` folder in the repo
2. Find the page you want to update, if the page is called **Azure Setup** then you will find a file named **azure-setup.md** in this folder
3. Open and edit your .md file
4. Save your file
5. Commit your changes and create a pull request