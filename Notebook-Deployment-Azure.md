# prerequisites
1. A user account. Need to provide a credit card for authentication.
2. Make sure that the SQL Server and the Web App services are always free.

After signing in
# Create a resource group
1) Go to the `Resource Group` service
2) Create a new resource group and give it a name (The subsciption should be automatically `Azure subscription` or `Free subscription` - that is Ok)
3) In the region select the region closest to you
4) After Azure creates the resource group click aa butto and go to the resource group
5) In the `Resource Group` dashboard click on the [Create] button to create a resource and search for the `Web App` in the `Market Place`
6) Select the `Web App` and click on the [Create] button.
7) Give the resource a name. It needs to be unique.
8) In the `Publish` options select [Code]
9) In the `Runtime stack` select the runtime you used for your application
10) `Operation System` - select [Windows] (not that important)
11) In the `region` select the one closest to you
12) In the `Pricing Plan` click on the `Create new`, give the plan a unique name and select the free plan (gives an hour of computing every day)
13) Click on the `Review and create` and then click on the `Create`
14) After the resource is deployed you can right click on the `Default domain` and select the `Open in new tab` option and you will see your application up and running

# Create a new account in Stripe for production
Click on the name of the account in the Sandbox mode, select `Other Accounts` and click on the `Create account` option
Then create a new account the same way you created the account for development. This is just to use different keys for production in Azure.

# Configuraing the Web App
1. In the Web App dashboard go to [Settings]
2. Under the [Settings] click on the `Environment variables` and in the Dashboard click on the [Add] button.
3. For stripe - in this project write StripeSettings in the Name field (It iss highly recommended to copy-paste from the appsettings to not make spelling mistakes!!!). Since it is not possible to define hierarchy, append '__<NextLevel>'. The result is: <StripeSettings__PublishableKey> (this is one field example. Should enter all the rest of the fields). The Deployment slot setting is only for paid versions so it is not relevant
4. After entering the [public] and [secret] keys from the new production account in stripe, add the [WhSecret] key, the same way.
5. Click <Apply> and <Confirm>

# Configuring Redis in Azure
6. Go to the `Connection strings` tab and click on the [Add] button.
7. In the [Name] field enter <Redis>. In the value, copy and paste the key as defined in the <appsettings.development.json> file.
8. Select [Custom] from the options in the [Types] field
9. Click [Apply] to the <Redis> connection string, Click [Apply] to the entire Connecation strings tab and click [Confirm]
10. Select the [Configuration] option under the [Settings] on the left sidebar
11. Make sure that the Web sockets checkbox is selected (on). If it is not (off) the SignalR will not work!

# Troubleshooting
**Click on thr Diagnose and solve problems in the left sidebar and under the Diagnostic Tools select the Application Event Logs. This will open a terminal with logs from the app we created**

# Creating a database in Azure
1) Go to the `Resource group` and select your resource group.
2) Click on the [Create] button and in the `Marketplace` search for [database] and from the options select the `SQL Database`
3) Make sure the subscription and resource group are correct.
4) Give the DB a name
5) In the [Server] field click on the `Create new` anchor:
   1) Give the server a name (unique)
   2) Select the same region you selected for the resource group. Otherwise this will cause latency
   3) In the `Authentication method`, select `Use SQL authentication` (the 3rd one)
   4) In the server admin login write: <appuser>
   5) In the [Password], enter a password under the Azure restrictions and `Confirm password`
   6) Click [Ok]
6) In the `Workload environment` field select the [Development] option
7) In the `Compute + storage` field select `Configure database` because the default option cost money
   1) In the Service tier select the Basic (For less demanding workloads) option (costs arround 6.5$ per month)
   2) Click Apply
8) In the `Backup storage redundancy` field select the `Locally-redundant backup storage` option
9) Click on the `Review and create` button and after reviewing and making sure everything is Ok, click on the `Create` button. The deployment takes a few minutes after which going to the resource will show the page for the database
10) Click on the `Show database connection strings` link in the `Connection strings` field.
11) Copy the `ADO.NET (SQL authentication)` to the clipboard
12) Go back to the `resource group` and click on the `App service` (Not `App service plan`!)
13) Go to the `Environment variables` under `Settings`, and then select the `Connection strings` tab.
14) Click on the [Add] button
    1)  In the name write the name of the connection string as defined in the app ([DefaultConnection]) in the <Name> field and paste the `ADO.NET (SQL authentication)` you copied in the <Value> field.
    2)  Change the phrase {your_password} to the password you defined earlier
    3)  In the <Type> field seklect the [SQLServer] option.
    4)  Click [Apply], [Apply] again, and [Confirm].
15) Go back to the `Resource group` and select the `SQL server` type you created (not `SQL database`)
16) There we need to confugure the network settings -> In the `Networking` field click on the `Show networking settings` link. There we need to provide access to this resource -> Select the Selected networks option.
17) Under the `Public access` tab, in the `Exception` section make sure the `Allow Azure services and resources to access this server` is checked!

# Publish the app to Azure
In VSCode make sure you have the Azure App Service extention and log in to your account. You will see all of your resources there.
In the terminal, in the API folder (this is that start project) write - `dotnet publish -c Release -o ./bin/Publish` and click [Enter] (-o is the target folder where all the published files will be). This will create a Publish folder under the bin folder in the API folder.
To publish it to Azure, right-click on the `Publish` folder and select the `Deploy to Web App...` option. This will display a list of the app services you created. Select the correct one. In the prompt window that asks "Are you sure you want to deploy..." click on the [Deploy] button. VSCode will ask if always to deploy the workspace to the selected server. Click on the `Skip for now` option unless you are sure that this workspace will always be deployed to the selected server.
The Publish will be deployed. At the end of the deployment VSCode will offer to open the app in the browser.

# In Stripe
1) Make sure you are in the correct account and click on the `Developers` icon at the bottom left of the page. In the list select the [Webhooks] option
2) Click on the Add destination button
3) Make sure the API version is the one you use
4) Select the desired events. In this project we only implemented the payment-intent-succeeded event. Then click on the Continue button.
5) Select the Webhook endpoint option and click on the Continue button.
6) Give a name to the destination. In the `Endpoint URL` enter the [URL] of the site in Azure and click on the `Create destination` button

# Continuous Integration
1) In `Azure`, in the `App Service`, in the left sidebar select the `Deployment` menu and in there select the `Deployment Center` option. In the Deployment Center we can configure how we want to deploy and build code from our preferred source and build provider. In the Source - select GitHub. If you need to authorize Azure in GitHub do so.
2) Select the GitHub organizarion and the repository that contains all the project files.
3) Select Main in the branch.
4) In the Authentication type, select User-assigned identity
5) In the subscription you will see your subscription in Azure (only have 1 subscription).
6) In the Identity select (New) or (Create new)
7) Click on the Preview file - you will see the workflow that will be set in the GutHub workflows so whenever we make a change in the code and commit it in GitHub it will run all the jobs in the script and basically deploy our updated application in Azure.
8) Before that - update the stripe public key to the skinet production key:
   1) Go to Stripe, to the production account you created and copy the Publishable key.
   2) Paste the copied key in the value of the stripePublishableKey in the environment.ts file in the client folder so it will match what we are using in the API in production.
   3) After this update - go to VSCode to the client folder and build it again to update the wwwroot folder in the API with the updated key