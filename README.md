# 🎓 UNI Care

[cite_start]**UNI Care** is a peer-to-peer (C2C) marketplace designed specifically for university students to buy, sell, or rent academic tools, equipment, and books[cite: 3]. [cite_start]The platform aims to reduce the financial burden on students by providing a secure environment for exchanging items that are often expensive and used for a limited time[cite: 4].

---

## 🔄 User Roles & Experience
[cite_start]UNI Care uses a Unified Account system with a specialized "Switch Mode" to cater to both sides of the marketplace[cite: 6]:

* [cite_start]**Borrower/Buyer Mode:** Primary focus is searching, browsing, and managing rentals/purchases with a buyer-centric theme and navigation[cite: 7].
* [cite_start]**Owner/Seller Mode:** Primary focus is managing listings, tracking earnings, and handling requests with a seller-centric dashboard and management tools[cite: 7].
* [cite_start]**Switching:** A prominent toggle in the profile/sidebar allows for an instant UI reconfiguration[cite: 7].

---

## 🚀 Core Features

### 🔐 1. Authentication & AI Verification
* [cite_start]**Sign-up/Login:** Support for any valid email address, Google account, or Phone Number[cite: 10].
* [cite_start]**AI-Powered OCR Verification:** Users must upload a valid proof of university enrollment[cite: 11, 13]. Accepted documents include:
    * [cite_start]Student ID Card [cite: 14]
    * [cite_start]University Nomination Card [cite: 15]
    * [cite_start]National ID Card (provided the faculty/university is explicitly mentioned) [cite: 16]
* [cite_start]The system utilizes OCR (Optical Character Recognition) to extract data: Full Name, University, Faculty, and Expiry Date (if applicable)[cite: 17]. 
* [cite_start]A "Verified Student" badge is awarded after successful verification to build trust[cite: 18].

### 🛍️ 2. Marketplace & Listings
* [cite_start]**Item Management:** Owners can list items with photos, descriptions, condition status, and pricing[cite: 20].
* [cite_start]**Pricing Options:** Fixed price for Sale, and flexible pricing (Daily, Weekly, or per Semester) for Rent[cite: 21, 22, 23].
* [cite_start]**Category Filtering:** Advanced filters for University, Faculty, Department, and Item Type (e.g., Engineering Tools, Medical Equipment)[cite: 24].
* [cite_start]**Favorites (Wishlist):** Users can save items for future reference or price tracking[cite: 25].

### 🤝 3. Secure Handover System (Trust Logic)
[cite_start]To ensure physical delivery and item security, the system uses a dual-verification code logic[cite: 26, 27]:
* [cite_start]**For Sales:** A single PIN/QR code is generated for the buyer; the seller verifies it upon delivery[cite: 28].
* [cite_start]**For Rentals (Start):** A code is generated for the renter to give to the owner upon receiving the item[cite: 29].
* [cite_start]**For Rentals (Return):** A new code is generated for the owner to give to the renter once the item is returned safely[cite: 30].
* [cite_start]*Note: Transaction status updates automatically upon code verification[cite: 31].*

### 💬 4. Communication & Notifications
* [cite_start]**In-App Chat:** Dedicated real-time chat for every transaction to maintain a record of agreements[cite: 33].
* [cite_start]**Smart Reminders:** Automated push notifications for rental return deadlines (48h, 24h, and same-day) and alerts for new booking requests and messages[cite: 34, 35, 36].

### 💳 5. Subscription & Payment System
[cite_start]The platform operates on a tiered subscription model for sellers/owners[cite: 38]:
* [cite_start]**Free Tier:** 1-2 active listings[cite: 39].
* [cite_start]**Basic Tier:** Up to 10 active listings[cite: 40].
* [cite_start]**Pro Tier:** Unlimited listings + "Featured" status[cite: 43].
* [cite_start]**Payment Integration:** Support for local gateways (e.g., Paymob, Fawry) for subscription payments[cite: 44].

### ⚖️ 6. Dispute Resolution System
[cite_start]A dedicated module to handle conflicts if an item is damaged or misrepresented[cite: 46]:
* [cite_start]**Ticket Filing:** Users can open a dispute within 24 hours of the return[cite: 47].
* [cite_start]**Evidence Upload:** Mandatory upload of "Before & After" photos and videos[cite: 48].
* [cite_start]**Audit Log:** Automatic attachment of Chat History and Handover timestamps for admin review[cite: 49].

### ⭐ 7. Reviews & Ratings
* [cite_start]**Dual-Rating System:** Both parties rate each other after the transaction is finalized[cite: 51].
* [cite_start]**Contextual Ratings:** Ratings are separated based on the role (Rating as a Seller vs. Rating as a Renter)[cite: 52].

---

## 🛠️ Technical Stack
* [cite_start]**Frontend:** React [cite: 54]
* [cite_start]**Backend:** .NET Core [cite: 54]
* [cite_start]**Database:** SQL Server [cite: 54]
* [cite_start]**AI/OCR:** Computer Vision [cite: 54]
* [cite_start]**Real-time:** SignalR [cite: 54]
