# Publishing this repo to GitHub

`gh` is optional. You can use the website or git remotes.

## Option A — GitHub website

1. Create a new empty repository on GitHub (no README).
2. In this folder:

```bash
cd ~/Projects/PaymentApi/PaymentApi
git init
git add .
git commit -m "Initial commit: basic PaymentApi learning project"
git branch -M main
git remote add origin https://github.com/<USERNAME>/<REPO>.git
git push -u origin main
```

## Option B — GitHub CLI

```bash
cd ~/Projects/PaymentApi/PaymentApi
git init
git add .
git commit -m "Initial commit: basic PaymentApi learning project"
gh repo create PaymentApi --public --source=. --remote=origin --push
```

Suggested repo description:

> Learning project: basic ASP.NET Core 8 Payment Service (mock provider, layered design). Not for production.
