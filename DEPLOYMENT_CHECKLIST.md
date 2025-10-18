# 🚀 Deployment Checklist - Maersk Hackathon

**Resource Group**: `app-container-tracking-maersk_group`  
**Time Remaining**: ~1.5 hours  
**Deployment Method**: Azure Portal (No GitHub connection)

---

## ✅ Part 1: Azure Event Hubs Setup (15 mins)

### Step 1: Create Event Hubs Namespace
1. Go to https://portal.azure.com
2. Search "Event Hubs" → Click **Event Hubs**
3. Click **+ Create**
4. Fill in:
   - **Resource Group**: `app-container-tracking-maersk_group`
   - **Namespace name**: `maersk-kafka-events` (if taken, try `maersk-kafka-events-2025`)
   - **Location**: Same as your backend (East US recommended)
   - **Pricing tier**: **Basic** (FREE)
   - **Throughput units**: 1
5. Click **Review + Create** → **Create**
6. ⏱️ Wait 2-3 minutes for deployment

### Step 2: Create Event Hub Topics
1. After deployment, click **Go to resource**
2. In left menu → **+ Event Hub** (under "Entities")
3. Create **first topic**:
   - Name: `port-events`
   - Partition Count: 2
   - Click **Create**
4. Click **+ Event Hub** again
5. Create **second topic**:
   - Name: `container-events`
   - Partition Count: 2
   - Click **Create**

### Step 3: Get Connection String ⚠️ IMPORTANT
1. In left menu → **Shared access policies** (under "Settings")
2. Click **RootManageSharedAccessKey**
3. **COPY** the **Connection string–primary key**
4. It looks like:
   ```
   Endpoint=sb://maersk-kafka-events.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=XXXXXXXXXXXXXX
   ```
5. **Save this in Notepad** - you'll need it next!

### Step 4: Extract Bootstrap Server from Connection String
From your connection string, extract the hostname:
- Connection string: `Endpoint=sb://maersk-kafka-events.servicebus.windows.net/;...`
- **Bootstrap Server** = `maersk-kafka-events.servicebus.windows.net:9093`

**Save both:**
- Full Connection String: `Endpoint=sb://...` 
- Bootstrap Server: `your-namespace.servicebus.windows.net:9093`

---

## ✅ Part 2: Configure Backend Environment Variables (5 mins)

### Step 5: Update Azure App Service Configuration

1. Go to Azure Portal
2. Search for your backend App Service (the one you deployed earlier)
3. In left menu → **Configuration** (under "Settings")
4. Click **+ New application setting** for each:

**Add these settings:**

| Name | Value |
|------|-------|
| `KAFKA_BOOTSTRAP_SERVERS` | `maersk-kafka-events.servicebus.windows.net:9093` |
| `EVENT_HUBS_CONNECTION_STRING` | `Endpoint=sb://maersk-kafka-events.servicebus.windows.net/;SharedAccessKeyName=...` (your full connection string) |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

5. Click **Save** at the top
6. Click **Continue** to restart the app

---

## ✅ Part 3: Deploy Updated Backend Code (10 mins)

### Step 6: Commit and Push Backend Changes

```powershell
# Navigate to repo root
cd "C:\Users\dhruv\Desktop\Company projects\Container-Tracking-and-Port-Operations-Maersk-Hackathon"

# Check what changed
git status

# Stage backend changes
git add backend/

# Commit
git commit -m "feat: integrate Azure Event Hubs for Kafka"

# Push to your deployment branch (or main if that's what you use)
git push origin feat/frontend-deployment
```

### Step 7: Deploy to Azure App Service

**If you're using local Git deployment:**
```powershell
# Add Azure remote if not already added
git remote add azure https://<your-deployment-username>@<your-app-name>.scm.azurewebsites.net/<your-app-name>.git

# Push to Azure
git push azure feat/frontend-deployment:main
```

**Or if using Azure Portal deployment:**
1. Go to your App Service
2. Left menu → **Deployment Center**
3. If connected to GitHub: sync latest changes
4. If using local Git: follow the Git credentials shown

⏱️ **Wait 3-5 minutes for deployment**

---

## ✅ Part 4: Frontend Deployment via ZIP (20 mins)

Since GitHub connection isn't working, we'll use **ZIP Deploy** method.

### Step 8: Build Frontend Production Bundle

```powershell
# Navigate to frontend
cd frontend

# Install dependencies (if not done)
npm install

# Build for production
npm run build
```

This creates a `dist/` folder with your production files.

### Step 9: Create Static Web App via Portal

1. Go to Azure Portal → **Create Resource**
2. Search **Static Web App** → Click **Create**
3. **Basics tab:**
   - Resource Group: `app-container-tracking-maersk_group`
   - Name: `maersk-port-operations`
   - Plan type: **Free**
   - Region: East US (or same as backend)
   - Deployment source: **Other** (since GitHub isn't connecting)
4. Click **Review + Create** → **Create**
5. ⏱️ Wait 2 minutes for deployment
6. Click **Go to resource**

### Step 10: Get Deployment Token

1. In your Static Web App → left menu → **Overview**
2. Look for **Manage deployment token** button at top
3. Click it → **Copy** the token
4. Save it in Notepad

### Step 11: Create Frontend Environment File

Create `frontend/.env.production` with your backend URL:

```env
VITE_API_BASE_URL=https://your-backend-name.azurewebsites.net
VITE_ENABLE_SIGNALR=true
```

**⚠️ Replace `your-backend-name` with your actual backend URL!**

### Step 12: Rebuild with Production Config

```powershell
# Still in frontend folder
npm run build
```

### Step 13: Deploy via Azure CLI or SWA CLI

**Option A: Using SWA CLI (Recommended)**
```powershell
# Install SWA CLI globally
npm install -g @azure/static-web-apps-cli

# Deploy
swa deploy ./dist --deployment-token "YOUR_DEPLOYMENT_TOKEN_FROM_STEP_10"
```

**Option B: Manual ZIP Upload**
1. Compress the `dist/` folder to `dist.zip`
2. Use Azure Portal → Static Web App → **Environment** → Upload

---

## ✅ Part 5: Update Backend CORS (5 mins)

### Step 14: Get Static Web App URL

1. In Azure Portal → Your Static Web App → **Overview**
2. Copy the **URL** (looks like `https://maersk-port-operations.azurestaticapps.net`)

### Step 15: Add CORS in Backend

You need to add this URL to backend CORS. Let me know your Static Web App URL and I'll help update the backend code.

---

## ✅ Part 6: Testing (10 mins)

### Step 16: Test Checklist

Visit your Static Web App URL: `https://maersk-port-operations.azurestaticapps.net`

**Test these features:**
- [ ] Landing page loads
- [ ] Login works
- [ ] Dashboard shows container data (no overflow!)
- [ ] Create a container
- [ ] Verify event appears in Event Streaming page
- [ ] Check Azure Event Hubs metrics show events

### Step 17: Check Event Hubs Metrics

1. Go to Azure Portal → Your Event Hubs Namespace
2. Left menu → **Metrics**
3. Add metric: **Incoming Messages**
4. You should see spikes when creating containers

---

## ✅ Part 7: Video Demo (10 mins)

### Step 18: Record Demo Video

**Use Windows Game Bar:**
1. Press `Win + G`
2. Click Record button
3. Demo flow:
   - Landing page
   - Login
   - Dashboard overview (show charts with no overflow!)
   - Create container
   - Show Event Streaming (live events)
   - Berth assignment
   - Admin panel

**Script:**
> "This is our Maersk Port Operations system built with Vue 3 and ASP.NET Core 8. We have real-time event streaming using Azure Event Hubs (Kafka-compatible), PostgreSQL database, and SignalR for live updates. Let me show you the container tracking dashboard..."

4. Save video → Upload to YouTube (unlisted) or Loom
5. Get shareable link

---

## 📝 Final Submission Checklist

- [ ] Event Hubs namespace created with 2 topics
- [ ] Backend deployed with Event Hubs connection string
- [ ] Frontend deployed to Static Web App
- [ ] CORS configured for frontend URL
- [ ] All features tested and working
- [ ] Video demo recorded and uploaded
- [ ] README updated with:
  - Live frontend URL
  - Demo video link
  - Tech stack summary

---

## 🆘 Troubleshooting

### Backend not connecting to Event Hubs
- Check App Service → Configuration → Verify both env vars are set
- Check App Service → Log stream for errors
- Verify connection string has correct format

### Frontend can't reach backend
- Check CORS is configured with frontend URL
- Verify `.env.production` has correct backend URL
- Check browser console for CORS errors

### Events not appearing
- Check Event Hubs metrics in Azure Portal
- Check backend logs for Kafka producer errors
- Verify topics exist: `port-events` and `container-events`

---

## Quick Reference

**Your Resources:**
- Resource Group: `app-container-tracking-maersk_group`
- Event Hubs Namespace: `maersk-kafka-events.servicebus.windows.net:9093`
- Static Web App: `https://maersk-port-operations.azurestaticapps.net`
- Backend: (your existing App Service URL)

**Connection String Format:**
```
Endpoint=sb://maersk-kafka-events.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=XXXXX
```

Good luck! 🚀
