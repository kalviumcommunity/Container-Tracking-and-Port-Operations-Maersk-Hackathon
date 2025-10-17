# 📖 User Manual

## Welcome to the Container Tracking & Port Operations System

This comprehensive user manual will guide you through all the features and functionality of the Maersk Container Tracking & Port Operations System. Whether you're a port manager, operator, or viewer, this manual provides step-by-step instructions for all system operations.

## 🚀 Getting Started

### System Access
**Production URL**: https://container-tracking.azurewebsites.net
**Test Environment**: https://staging-container-tracking.azurewebsites.net

### Supported Browsers
- **Chrome**: Version 90+ (Recommended)
- **Firefox**: Version 88+
- **Safari**: Version 14+
- **Edge**: Version 90+

### Account Types & Permissions
| Role | Access Level | Key Permissions |
|------|-------------|-----------------|
| **Admin** | Full system access | Manage all resources, user management, system configuration |
| **Port Manager** | Port-level operations | Manage containers, ships, berths within assigned ports |
| **Operator** | Daily operations | Container operations, berth assignments, movement tracking |
| **Viewer** | Read-only access | View dashboards, reports, and operational data |

## 🔐 Login & Authentication

### First-Time Login
1. **Navigate** to the application URL
2. **Click** "Login" in the top navigation
3. **Enter** your provided credentials:
   - Username or Email
   - Password
4. **Click** "Sign In"
5. **Complete** profile setup if prompted

### Password Reset
1. **Click** "Forgot Password?" on login screen
2. **Enter** your registered email address
3. **Check** your email for reset instructions
4. **Follow** the link to create a new password
5. **Login** with your new credentials

### Two-Factor Authentication (If Enabled)
1. **Enter** username and password
2. **Open** your authenticator app (Google Authenticator, Authy)
3. **Enter** the 6-digit code
4. **Click** "Verify"

## 🏠 Dashboard Overview

### Main Dashboard
The dashboard provides a comprehensive overview of port operations and key performance indicators.

#### Key Metrics Cards
- **Active Containers**: Total containers currently in the system
- **Ships in Port**: Number of ships currently docked
- **Available Berths**: Free berths ready for assignment
- **Today's Movements**: Container movements completed today

#### Interactive Charts
- **Container Throughput**: Daily/weekly/monthly container processing
- **Berth Utilization**: Real-time berth occupancy rates
- **Ship Schedule**: Upcoming arrivals and departures
- **Performance Trends**: Historical operational metrics

#### Quick Actions
- **Add New Container**: Quick container registration
- **Assign Berth**: Immediate berth assignment
- **Record Movement**: Log container movement
- **Generate Report**: Create operational reports

### Navigation Menu
- **🏠 Dashboard**: Main overview page
- **📦 Containers**: Container management and tracking
- **🚢 Ships**: Ship information and scheduling
- **⚓ Berths**: Berth operations and assignments
- **📊 Analytics**: Detailed reports and analytics
- **👥 Users**: User management (Admin only)
- **⚙️ Settings**: System configuration

## 📦 Container Management

### Viewing Containers

#### Container List View
1. **Navigate** to **Containers** → **All Containers**
2. **Use filters** to find specific containers:
   - Container Number
   - Type (DRY, REEFER, TANK, OPEN_TOP)
   - Status (Available, In Transit, Assigned, Damaged)
   - Location/Port
   - Date Range

#### Container Details
1. **Click** on any container row to view details
2. **Review** comprehensive information:
   - Basic Information (Number, Type, Size, Weight)
   - Current Location and Status
   - Movement History
   - Associated Ship (if applicable)
   - Maintenance Records

#### Search & Filters
```
Search Options:
- Quick Search: Enter container number or partial match
- Advanced Filter: Multiple criteria selection
- Date Range: Movement or creation date filters
- Location Filter: Port or berth specific containers
```

### Adding New Containers

#### Manual Container Entry
1. **Click** "Add New Container" button
2. **Fill** required information:
   - **Container Number**: Unique identifier (e.g., MSKU1234567)
   - **Type**: Select from dropdown (DRY/REEFER/TANK/OPEN_TOP)
   - **Size**: Container dimensions (20FT/40FT/45FT)
   - **Weight**: Tare weight in tons
   - **Current Location**: Port/Berth assignment
3. **Optional fields**:
   - Owner information
   - Special handling requirements
   - Condition notes
4. **Click** "Save Container"

#### Bulk Container Import
1. **Navigate** to **Containers** → **Import**
2. **Download** the template CSV file
3. **Fill** the template with container data
4. **Upload** the completed file
5. **Review** import preview
6. **Confirm** import to add all containers

### Container Operations

#### Recording Container Movements
1. **Select** container from list
2. **Click** "Record Movement"
3. **Fill** movement details:
   - **From Location**: Current position
   - **To Location**: Destination
   - **Movement Type**: Discharge/Load/Transfer
   - **Date & Time**: When movement occurred
   - **Notes**: Additional information
4. **Save** movement record

#### Container Status Updates
1. **Open** container details
2. **Click** "Update Status"
3. **Select** new status:
   - Available: Ready for assignment
   - In Transit: Currently moving
   - Assigned: Allocated to ship/berth
   - Maintenance: Under repair
   - Damaged: Requires inspection
4. **Add** status change reason
5. **Confirm** update

#### Container Assignment to Ships
1. **Open** container details
2. **Click** "Assign to Ship"
3. **Select** ship from available list
4. **Choose** position on ship
5. **Set** assignment date
6. **Confirm** assignment

## 🚢 Ship Management

### Ship Information

#### Viewing Ship Details
1. **Navigate** to **Ships** section
2. **Select** ship from list
3. **Review** ship information:
   - Basic Details (Name, IMO Number, Flag, Type)
   - Specifications (Length, Width, Capacity)
   - Current Status and Location
   - Berth Assignment (if applicable)
   - Container Manifest

#### Ship Status Indicators
- **🟢 In Port**: Currently docked at berth
- **🟡 Arriving**: Scheduled to arrive soon
- **🔴 Departed**: Recently left port
- **⚫ At Sea**: En route to/from port

### Ship Operations

#### Berth Assignment
1. **Select** ship requiring berth
2. **Click** "Assign Berth"
3. **Choose** available berth from list
4. **Set** assignment details:
   - Scheduled Arrival Date & Time
   - Scheduled Departure Date & Time
   - Assignment Priority
   - Special Requirements
5. **Confirm** assignment

#### Recording Ship Arrivals
1. **Navigate** to expected arrivals
2. **Find** ship in arrival schedule
3. **Click** "Record Arrival"
4. **Enter** actual arrival details:
   - Actual Arrival Time
   - Berth Assignment
   - Condition Notes
   - Any Delays/Issues
5. **Update** ship status to "In Port"

#### Managing Ship Departures
1. **Select** ship ready for departure
2. **Click** "Process Departure"
3. **Verify** all containers loaded/discharged
4. **Complete** departure checklist:
   - Final container count
   - Documentation complete
   - Port charges settled
   - Clearance obtained
5. **Record** actual departure time

### Container Loading/Unloading

#### Container Discharge Operations
1. **Open** ship container manifest
2. **Select** containers to discharge
3. **For each container**:
   - Verify container condition
   - Record discharge location
   - Update container status
   - Note any damage/issues
4. **Complete** discharge process

#### Container Loading Operations
1. **Select** ship for loading
2. **Choose** containers from available pool
3. **Assign** container positions on ship
4. **Verify** weight distribution
5. **Update** container and ship status
6. **Generate** loading manifest

## ⚓ Berth Operations

### Berth Management

#### Berth Overview
1. **Navigate** to **Berths** section
2. **View** berth layout and status:
   - **Green**: Available for assignment
   - **Yellow**: Assigned/Reserved
   - **Red**: Occupied
   - **Gray**: Under maintenance

#### Berth Details
- **Physical Specifications**: Length, width, depth
- **Equipment Available**: Cranes, loading facilities
- **Current Occupant**: Assigned ship (if any)
- **Utilization History**: Historical usage data
- **Maintenance Schedule**: Upcoming maintenance

### Berth Assignment Operations

#### Creating New Assignments
1. **Click** "New Assignment" button
2. **Select** ship from dropdown
3. **Choose** available berth
4. **Set** schedule:
   - Arrival Date & Time
   - Departure Date & Time
   - Buffer Time (if needed)
5. **Add** special requirements:
   - Crane requirements
   - Security needs
   - Special handling
6. **Save** assignment

#### Modifying Existing Assignments
1. **Find** assignment in list
2. **Click** "Edit" button
3. **Modify** required fields:
   - Timing adjustments
   - Berth changes (if available)
   - Special requirements
4. **Add** modification reason
5. **Update** assignment

#### Managing Assignment Conflicts
1. **System** will highlight conflicts automatically
2. **Review** conflicting assignments
3. **Choose resolution**:
   - Adjust timing
   - Reassign to different berth
   - Negotiate with stakeholders
4. **Update** affected assignments
5. **Notify** relevant parties

### Berth Utilization Monitoring

#### Real-Time Status
- **Current Occupancy**: Live berth status
- **Upcoming Arrivals**: Next 24/48 hours
- **Departure Schedule**: Expected departures
- **Utilization Rate**: Percentage usage metrics

#### Historical Analysis
- **Usage Patterns**: Peak times identification
- **Efficiency Metrics**: Average turnaround times
- **Revenue Analysis**: Berth-generated income
- **Optimization Opportunities**: Improvement suggestions

## 📊 Analytics & Reporting

### Dashboard Analytics

#### Key Performance Indicators (KPIs)
- **Container Throughput**: Containers processed per period
- **Berth Utilization**: Average occupancy percentage
- **Ship Turnaround Time**: Average dock-to-departure duration
- **Operational Efficiency**: Tasks completed vs. planned
- **Revenue Metrics**: Financial performance indicators

#### Interactive Charts
1. **Select** chart type from dropdown
2. **Choose** date range using calendar
3. **Apply** filters (port, ship type, container type)
4. **Export** chart as PDF/PNG if needed

### Custom Reports

#### Creating Reports
1. **Navigate** to **Analytics** → **Custom Reports**
2. **Select** report template or create new
3. **Configure** report parameters:
   - Data sources (containers, ships, berths)
   - Date ranges
   - Filters and grouping
   - Output format
4. **Preview** report layout
5. **Generate** and save report

#### Scheduled Reports
1. **Create** or edit existing report
2. **Click** "Schedule Report"
3. **Set** schedule parameters:
   - Frequency (daily, weekly, monthly)
   - Time of generation
   - Email recipients
   - Format preferences
4. **Activate** scheduled reporting

#### Available Report Types
- **Operational Reports**: Daily/weekly operations summary
- **Financial Reports**: Revenue and cost analysis
- **Performance Reports**: Efficiency and productivity metrics
- **Compliance Reports**: Regulatory compliance status
- **Maintenance Reports**: Equipment and facility maintenance

### Data Export Options

#### Export Formats
- **Excel (.xlsx)**: For detailed data analysis
- **CSV**: For database imports
- **PDF**: For presentations and printing
- **JSON**: For API integration

#### Export Process
1. **Select** data to export (containers, ships, berths)
2. **Apply** filters if needed
3. **Choose** export format
4. **Click** "Export Data"
5. **Download** file when ready

## 👥 User Management (Admin Only)

### User Account Operations

#### Creating New Users
1. **Navigate** to **Users** → **Add User**
2. **Fill** user information:
   - Full Name
   - Email Address
   - Username
   - Initial Password
   - Role Assignment
   - Port Assignment (if applicable)
3. **Set** account permissions
4. **Send** welcome email
5. **Save** user account

#### Managing Existing Users
1. **Find** user in user list
2. **Click** "Edit" to modify:
   - Personal information
   - Role assignments
   - Permission levels
   - Account status (active/inactive)
3. **Password** reset if needed
4. **Save** changes

#### Role Management
- **Admin**: Full system access and user management
- **Port Manager**: Port-specific operations and limited user management
- **Operator**: Daily operations within assigned areas
- **Viewer**: Read-only access to assigned data

### Permission Configuration

#### Role-Based Permissions
```
Admin Permissions:
✅ User Management
✅ System Configuration
✅ All Port Operations
✅ Full Analytics Access
✅ Data Export/Import

Port Manager Permissions:
✅ Port-Specific Operations
✅ Limited User Management
✅ Port Analytics
✅ Data Export (Port Only)
❌ System Configuration

Operator Permissions:
✅ Container Operations
✅ Ship Operations
✅ Berth Assignments
❌ User Management
❌ System Configuration

Viewer Permissions:
✅ View Dashboards
✅ View Reports
❌ Data Modification
❌ User Management
❌ System Configuration
```

## ⚙️ Settings & Configuration

### Profile Settings

#### Personal Profile
1. **Click** user avatar in top-right corner
2. **Select** "Profile Settings"
3. **Update** information:
   - Display Name
   - Email Address
   - Time Zone
   - Language Preference
   - Notification Settings
4. **Save** changes

#### Password Change
1. **Go** to Profile Settings
2. **Click** "Change Password"
3. **Enter** current password
4. **Enter** new password (twice)
5. **Confirm** password change

### System Preferences

#### Display Settings
- **Theme**: Light/Dark mode selection
- **Language**: Interface language selection
- **Date Format**: Regional date formatting
- **Time Zone**: Local time zone setting
- **Number Format**: Regional number formatting

#### Notification Settings
- **Email Notifications**: Enable/disable email alerts
- **Browser Notifications**: Push notification preferences
- **Alert Types**: Select which events trigger notifications
- **Notification Frequency**: Immediate/digest/weekly options

### Application Configuration (Admin Only)

#### System Settings
- **Company Information**: Organization details
- **Default Settings**: System-wide defaults
- **Integration Settings**: External system connections
- **Backup Configuration**: Data backup preferences
- **Security Settings**: Password policies, session timeouts

## 🆘 Help & Support

### Common Issues & Solutions

#### Login Problems
**Issue**: Cannot log in to system
**Solutions**:
1. Verify username/email and password
2. Check caps lock status
3. Try password reset
4. Clear browser cache and cookies
5. Contact system administrator

#### Performance Issues
**Issue**: System running slowly
**Solutions**:
1. Check internet connection
2. Close other browser tabs
3. Clear browser cache
4. Try different browser
5. Report persistent issues to support

#### Data Not Loading
**Issue**: Containers/ships not displaying
**Solutions**:
1. Refresh the page (F5)
2. Check filter settings
3. Verify permissions for data access
4. Contact support if problem persists

### Contact Information

#### Technical Support
- **Email**: support@containertracking.com
- **Phone**: +1-800-MAERSK-1
- **Hours**: 24/7 for critical issues, 8 AM - 6 PM for general support

#### Emergency Contacts
- **Production Issues**: emergency@containertracking.com
- **Security Incidents**: security@containertracking.com
- **Data Issues**: data-support@containertracking.com

### Training Resources

#### Video Tutorials
- **Getting Started**: 15-minute system overview
- **Container Management**: 30-minute detailed walkthrough
- **Berth Operations**: 25-minute operational guide
- **Analytics & Reporting**: 20-minute reporting tutorial

#### Documentation Library
- **Quick Reference Cards**: Printable operation guides
- **Feature Updates**: Latest system enhancements
- **Best Practices**: Operational efficiency tips
- **FAQs**: Frequently asked questions

### Feedback & Suggestions

#### Providing Feedback
1. **Click** "Feedback" button (bottom-right corner)
2. **Select** feedback type:
   - Bug Report
   - Feature Request
   - General Feedback
   - Improvement Suggestion
3. **Fill** detailed description
4. **Add** screenshots if helpful
5. **Submit** feedback

#### Feature Requests
When requesting new features, please include:
- **Business Justification**: Why this feature is needed
- **Use Case**: How it would be used
- **Priority Level**: How important this is
- **Alternatives**: Current workarounds being used

We appreciate your feedback and continuously work to improve the system based on user input. Thank you for using the Container Tracking & Port Operations System!