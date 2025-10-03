# Enhanced Business Data Seeding Guide

## 🎯 Overview

Your Azure PostgreSQL Container Tracking application now supports **comprehensive business data seeding** with realistic, production-like test data. This enhancement adds hundreds of realistic records to make your API demonstrations and testing much more impressive.

## 📊 What Gets Seeded

### 🏗️ **25 Major World Ports**
- **European Ports:** Copenhagen, Rotterdam, Hamburg, Antwerp, Valencia, Genoa, Southampton, Le Havre
- **Asian Ports:** Shanghai, Singapore, Busan, Hong Kong, Tokyo, Kaohsiung  
- **American Ports:** Los Angeles, Long Beach, New York, Miami, Vancouver, Santos
- **Middle East/Africa:** Dubai, Suez, Durban, Jeddah

### 🚢 **60+ Ships from Major Shipping Lines**
- **Maersk Fleet:** Maersk Edinburgh, Madrid Maersk, Maersk Honam, etc.
- **MSC Fleet:** MSC Gulsun, MSC Oscar, MSC Maya, etc.
- **COSCO Fleet:** COSCO Shipping Universe, COSCO Shipping Aries, etc.
- **Evergreen Fleet:** Ever Given, Ever Ace, Ever Globe, etc.
- **Other Lines:** HMM, OOCL, CMA CGM, ONE, Hapag-Lloyd, Yang Ming, ZIM

### 📦 **300 Diverse Containers**
- **Container Types:** Dry, Refrigerated, Hazardous, Liquid, Open Top, Flat Rack, Tank, ISO Tank
- **Cargo Types:** Electronics, Automotive Parts, Food & Beverages, Pharmaceuticals, Raw Materials, etc.
- **Realistic Status:** Empty, Loaded, In Transit, Loading, Unloading, Inspected, Under Repair, etc.
- **Global Locations:** Distributed across all major ports and "At Sea" locations

### ⚓ **120+ Berth Assignments**
- **Realistic Timelines:** Assignments spanning up to 3 months
- **Varied Duration:** 2 hours to 1 week berth occupancy
- **Status Distribution:** 75% released, 25% currently assigned

### 🔗 **80+ Ship-Container Operations**
- **Loading Operations:** Containers loaded onto ships within the last month
- **Realistic Distribution:** 20% containers not assigned to ships (in ports/warehouses)

## 🚀 How to Use

### Option 1: PowerShell Script (Recommended)
```powershell
cd backend
.\seed-enhanced-data.ps1
```

This script will:
1. ✅ Build and start your API
2. ✅ Seed all enhanced business data via API endpoint
3. ✅ Show you the final data summary
4. ✅ Keep the API running for testing

### Option 2: API Endpoint
If your API is already running:

```bash
# Check current data status
GET http://localhost:5221/api/seed/status

# Trigger enhanced business data seeding
POST http://localhost:5221/api/seed/enhanced-business-data
```

### Option 3: Direct Integration
Add to your `Program.cs` or call manually:

```csharp
using Backend.Data.Seeding;

// After your existing seeding
await EnhancedDataSeeder.SeedAsync(context);
```

## 🎯 Benefits for Testing & Demos

### **More Realistic API Responses**
- Instead of 6 ports → **25 major world ports**
- Instead of 12 ships → **60+ ships from major shipping lines** 
- Instead of 50 containers → **300 containers with diverse cargo**

### **Better Data Distribution**
- **Global Coverage:** Ports across 6 continents
- **Industry Realistic:** Actual shipping company names and container types
- **Time-based Data:** Realistic timestamps spanning months
- **Status Variety:** Multiple container and berth statuses

### **Enhanced Demo Experience**
- **Pagination Testing:** Enough data to test pagination properly
- **Search & Filter:** Rich data for complex search scenarios
- **Realistic Workflows:** Demonstrate real-world port operations
- **Visual Appeal:** Professional data that looks production-ready

## 🔐 Authentication Data (Already Seeded)

The enhanced seeding **does not affect** your existing authentication system:

- **Admin User:** `admin@example.com` / `admin123`
- **Roles:** Admin, PortManager, Operator, Viewer
- **Permissions:** 21 granular permissions across all operations

## 📊 Data Verification

After seeding, verify your data with these API calls:

```bash
# Count verification
GET /api/seed/status

# Browse the enhanced data
GET /api/ports          # 25 ports
GET /api/ships          # 60+ ships  
GET /api/containers     # 300 containers
GET /api/berths         # Berths across all ports
```

## ⚠️ Important Notes

1. **Idempotent Seeding:** Safe to run multiple times - won't create duplicates
2. **Azure Compatible:** Fully tested with Azure PostgreSQL
3. **Performance:** Seeding takes 30-60 seconds depending on your Azure region
4. **Data Consistency:** All relationships properly maintained (Foreign Keys)
5. **Production Ready:** Professional, realistic data suitable for client demos

## 🎉 Result

Your Container Tracking API now has **production-quality sample data** that showcases the full capabilities of your port operations management system! Perfect for demos, testing, and development.