# 🎓 UNI Care

**UNI Care** is a peer-to-peer (C2C) marketplace designed specifically for university students to buy, sell, or rent academic tools, equipment, and books. The platform aims to reduce the financial burden on students by providing a secure environment for exchanging items that are often expensive and used for a limited time.

---

## 🎨 UI/UX Design

You can explore the complete user interface and user experience design for the UNI Care project on Figma:
🔗 **[View UNI Care Design on Figma](https://www.figma.com/design/hFdFDRsoLaOpLgjDULQLMO/UniCare?node-id=0-1&p=f&t=CtIQ6bdf83AKwGJp-0)**

---

## 🔄 User Roles & Experience

UNI Care uses a Unified Account system with a specialized "Switch Mode" to cater to both sides of the marketplace:

- **Borrower/Buyer Mode:** Primary focus is searching, browsing, and managing rentals/purchases with a buyer-centric theme and navigation.
- **Owner/Seller Mode:** Primary focus is managing listings, tracking earnings, and handling requests with a seller-centric dashboard and management tools.
- **Switching:** A prominent toggle in the profile/sidebar allows for an instant UI reconfiguration.

---

## 🚀 Core Features

### 🔐 1. Authentication & AI Verification

- **Sign-up/Login:** Support for any valid email address, Google account, or Phone Number.
- **AI-Powered OCR Verification:** Users must upload a valid proof of university enrollment (Student ID, Nomination Card, or National ID).
- **Smart Data Extraction:** The system utilizes OCR to extract Full Name, University, Faculty, and Expiry Date.
- **Trust Building:** A "Verified Student" badge is awarded after successful verification.

### 🛍️ 2. Marketplace & Listings

- **Item Management:** Owners can list items with photos, descriptions, condition status, and pricing.
- **Pricing Options:** Fixed price for Sale, and flexible pricing (Daily, Weekly, or per Semester) for Rent.
- **Category Filtering:** Advanced filters for University, Faculty, Department, and Item Type.
- **Favorites (Wishlist):** Users can save items for future reference or price tracking.

### 🤝 3. Secure Handover System (Trust Logic)

To ensure physical delivery and item security, the system uses a dual-verification code logic:

- **For Sales:** A single PIN/QR code is generated for the buyer; the seller verifies it upon delivery.
- **For Rentals (Start):** A code is generated for the renter to give to the owner upon receiving the item.
- **For Rentals (Return):** A new code is generated for the owner to give to the renter once the item is returned safely.

### 💬 4. Communication & Notifications

- **In-App Chat:** Dedicated real-time chat for every transaction to maintain a record of agreements.
- **Smart Reminders:** Automated push notifications for rental return deadlines and alerts for new booking requests.

### 💳 5. Subscription & Payment System

- **Early Adoption Phase:** To encourage community growth and initial listings, the platform will be **entirely free** for all users during the launch period.
- **Future Tiered Plans:** A subscription model will be activated in later stages for sellers/owners:
  - **Free Tier:** 1-2 active listings.
  - **Basic Tier:** Up to 10 active listings.
  - **Pro Tier:** Unlimited listings + "Featured" status.
- **Payment Integration:** Support for local gateways (e.g., Paymob, Fawry) will be enabled for future subscription payments.

### ⚖️ 6. Dispute Resolution System

- **Ticket Filing:** Users can open a dispute within 24 hours of the return if an item is damaged or misrepresented.
- **Evidence Upload:** Mandatory upload of "Before & After" photos and videos.
- **Audit Log:** Automatic attachment of Chat History and Handover timestamps for admin review.

### ⭐ 7. Reviews & Ratings

- **Dual-Rating System:** Both parties rate each other after the transaction is finalized.
- **Contextual Ratings:** Ratings are separated based on the role (Rating as a Seller vs. Rating as a Renter).

---

## 🛠️ Technical Stack

- **Frontend:** Next js (TypeScript, Tailwind CSS)
- **Backend:** .NET Core (C#)
- **Database:** SQL Server
- **AI/OCR:** Computer Vision
- **Real-time:** SignalR
