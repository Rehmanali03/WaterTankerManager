# How to Upload Your Project to GitHub

Since the automated tools are being cancelled, follow these manual steps in your terminal.

## Step 1: Initialize Git (Fixing the "fatal: not a git repository" error)
Copy and paste these commands into your terminal one by one:

```powershell
# 1. Enter the project directory
cd "c:\Users\Cam. Computer Lab\.gemini\antigravity\scratch\WaterTankerManager"

# 2. Initialize the repository
git init

# 3. Configure your local identity (Required for committing)
git config user.email "you@example.com"
git config user.name "YourName"

# 4. Save your files
git add .
git commit -m "Initial commit"
```

## Step 2: Connect to GitHub
1.  Go to [GitHub.com](https://github.com) and sign in.
2.  Create a **New Repository**.
    *   Name: `WaterTankerManager`
    *   **Uncheck** "Add a README file" (we already have one).
    *   Click **Create repository**.

## Step 3: Push and Connect Account
On the GitHub page, look for the section **"…or push an existing repository from the command line"**. Copy those lines. They look like this:

```powershell
git remote add origin https://github.com/YOUR_USERNAME/WaterTankerManager.git
git branch -M main
git push -u origin main
```

**When you run the last command (`git push ...`), a browser window will pop up asking you to authorize "Git Credential Manager" or sign in to GitHub.** 
*   Click "Sign In with Browser".
*   This will automatically "connect" your GitHub account.

## Summary of Commands to Copy
If you just want to copy-paste everything at once:

```powershell
cd "c:\Users\Cam. Computer Lab\.gemini\antigravity\scratch\WaterTankerManager"
git init
git config user.email "admin@water.com"
git config user.name "Admin"
git add .
git commit -m "Ready to upload"
```
*(After this, run the `git remote add ...` command from your GitHub page)*
