# TripNest 🌍

[Live Demo](https://tripnest-r14u.onrender.com/) | [GitHub Repository](https://github.com/your-username/tripnest)

## Overview
TripNest is a modern, full-stack travel and tour booking platform built with ASP.NET Core MVC and PostgreSQL. It connects travelers with verified local agencies, allowing users to explore, book, and review unique travel experiences worldwide. Agencies can manage tours, bookings, and customer reviews through a dedicated dashboard with cloud-based image management.

---

## ✨ Features

### 🎒 For Travelers
- **Browse & Search Tours:** Explore featured and all available tours by destination, date, and price range
- **User Registration & Login:** Secure sign-up and sign-in with profile management
- **Profile Dashboard:** Comprehensive dashboard to manage profile, view bookings, and track reviews
- **Book Tours:** Book tours with custom guest count, preferred dates, and special requests
- **Booking Management:** Real-time booking status tracking (pending, confirmed, completed, cancelled)
- **Leave Reviews:** Submit detailed reviews and star ratings for completed tours
- **Contact Support:** Direct messaging to TripNest support via integrated contact form

### 🏢 For Agencies
- **Agency Login:** Secure authentication system for travel agencies
- **Agency Dashboard:** Centralized overview with analytics and quick access to management features
- **Manage Tours:** Full CRUD operations for tour listings with cloud image management
- **Image Upload:** Seamless image upload and optimization via Cloudinary integration
- **Manage Bookings:** View, filter, and update booking statuses with detailed booking information
- **View Reviews:** Monitor customer feedback and ratings with analytics
- **Contact Messages:** Centralized inbox for customer inquiries and support requests

### 🌟 General Features
- **Responsive UI:** Mobile-first design with Bootstrap 5 and custom CSS animations
- **Cloud Image Management:** Cloudinary integration for optimized image storage and delivery
- **Authentication:** Secure cookie-based authentication for users and agencies
- **PostgreSQL Database:** Robust, scalable data storage with Entity Framework Core
- **MVC Architecture:** Clean separation of concerns for maintainability and scalability
- **Environment-based Configuration:** Secure credential management for different deployment environments

---

## 🛠 Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** PostgreSQL with Entity Framework Core
- **Authentication:** Cookie-based Authentication (ASP.NET Core)
- **Cloud Storage:** Cloudinary for image management and optimization
- **Email Service:** SendGrid integration (configured)

### Frontend
- **View Engine:** Razor Views with C#
- **CSS Framework:** Bootstrap 5 with custom responsive design
- **JavaScript:** Vanilla JS with jQuery for enhanced interactivity
- **Icons:** Bootstrap Icons for consistent UI elements

### DevOps & Deployment
- **Hosting:** Render.com with automatic deployments
- **Database Hosting:** PostgreSQL on Render
- **Environment Management:** Secure environment variable configuration
- **Version Control:** Git with GitHub integration

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (local development)
- [Cloudinary Account](https://cloudinary.com/) (for image management)

### 📋 Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/tripnest.git
   cd TripNest
   ```

2. **Install dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure environment variables:**
   
   Create a `.env` file in the root directory:
   ```env
   # Database Configuration
   ConnectionStrings__DefaultConnection=your_postgresql_connection_string
   
   # Cloudinary Configuration
   Cloudinary__CloudName=your_cloudinary_cloud_name
   Cloudinary__ApiKey=your_cloudinary_api_key
   Cloudinary__ApiSecret=your_cloudinary_api_secret
   
   # Logging Configuration
   Logging__LogLevel__Default=Information
   Logging__LogLevel__Microsoft_AspNetCore=Warning
   ```

4. **Apply database migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

6. **Access the application:**
   - Navigate to `https://localhost:7244` or the port shown in your terminal
   - The application will automatically create the database schema on first run

### 🔐 Environment Variables Configuration

| Variable | Description | Required |
|----------|-------------|----------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | ✅ |
| `Cloudinary__CloudName` | Cloudinary cloud name | ✅ |
| `Cloudinary__ApiKey` | Cloudinary API key | ✅ |
| `Cloudinary__ApiSecret` | Cloudinary API secret | ✅ |
| `Logging__LogLevel__Default` | Application logging level | ❌ |

---

## 📁 Project Structure

```
TripNest/
├── Controllers/                 # MVC Controllers
│   ├── AccountController.cs     # User authentication & profile
│   ├── AgencyController.cs      # Agency management & dashboard
│   ├── BookingController.cs     # Tour booking operations
│   ├── ContactController.cs     # Contact form handling
│   ├── HomeController.cs        # Public pages & tour display
│   ├── ReviewController.cs      # Review management
│   └── TourController.cs        # Tour CRUD operations
├── Models/                      # Data models & ViewModels
│   ├── ViewModels/             # View-specific models
│   ├── User.cs                 # User entity
│   ├── Agency.cs               # Agency entity
│   ├── Tour.cs                 # Tour entity
│   ├── Booking.cs              # Booking entity
│   └── Review.cs               # Review entity
├── Views/                       # Razor view templates
│   ├── Account/                # User account views
│   ├── Agency/                 # Agency dashboard views
│   ├── Booking/                # Booking-related views
│   ├── Home/                   # Public pages
│   ├── Review/                 # Review submission views
│   ├── Tour/                   # Tour management views
│   └── Shared/                 # Shared layouts & partials
├── Data/                       # Database context & migrations
│   └── ApplicationDbContext.cs # EF Core DbContext
├── Services/                   # Business logic services
│   ├── CloudinaryService.cs   # Image upload service
│   └── EmailService.cs        # Email service
├── wwwroot/                    # Static assets
│   ├── css/                    # Custom stylesheets
│   ├── js/                     # JavaScript files
│   ├── images/                 # Static images
│   └── videos/                 # Promotional videos
├── Migrations/                 # EF Core database migrations
├── appsettings.json           # Application configuration
├── Program.cs                 # Application entry point
└── render.yaml               # Deployment configuration
```

---

## 📊 Database Schema

### Core Entities
- **User** → Travelers who book tours and leave reviews
- **UserProfile** → Extended user information and preferences
- **Agency** → Travel agencies that manage tours
- **Tour** → Travel experiences offered by agencies
- **Booking** → Reservations made by users for specific tours
- **Review** → User feedback and ratings for completed tours
- **ContactMessage** → Customer inquiries submitted via contact form

### Relationships
- User ↔ UserProfile (One-to-One)
- User ↔ Booking (One-to-Many)
- User ↔ Review (One-to-Many)
- Agency ↔ Tour (One-to-Many)
- Tour ↔ Booking (One-to-Many)
- Booking ↔ Review (One-to-One)

---

## ☁️ Cloudinary Integration

TripNest uses Cloudinary for comprehensive image management:

### Features
- **Automatic Image Optimization:** Images are automatically resized to 1200x800px with optimal quality
- **Format Conversion:** Automatic WebP conversion for better performance
- **CDN Delivery:** Global content delivery network for fast image loading
- **Secure Upload:** Direct server-side upload with validation
- **Image Transformation:** On-the-fly image modifications via URL parameters

### Configuration
```csharp
// Image upload with transformation
var uploadParams = new ImageUploadParams()
{
    File = new FileDescription(fileName, stream),
    Folder = "tours",
    Transformation = new Transformation()
        .Quality("auto")
        .FetchFormat("auto")
        .Width(1200)
        .Height(800)
        .Crop("fill")
        .Gravity("auto")
};
```

### Benefits
- ✅ **Reduced Server Load:** No local image storage required
- ✅ **Better Performance:** Global CDN with automatic optimization
- ✅ **Scalability:** Handle unlimited image uploads
- ✅ **SEO Friendly:** Faster page load times improve search rankings

---

## 🚀 Deployment

### Render.com Deployment

1. **Connect your GitHub repository** to Render
2. **Set environment variables** in Render dashboard:
   - `DATABASE_URL` (PostgreSQL connection)
   - `CLOUDINARY_CLOUD_NAME`
   - `CLOUDINARY_API_KEY`
   - `CLOUDINARY_API_SECRET`
3. **Deploy automatically** on every push to main branch

### Manual Deployment
```bash
# Build for production
dotnet publish -c Release -o out

# Run the published application
dotnet out/TripNest.dll
```

---

## 🔧 API Endpoints

### Public Routes
- `GET /` - Homepage with featured tours
- `GET /Home/Packages` - All available tours
- `GET /Tour/Details/{id}` - Tour details page
- `GET /Tour/Search` - Search tours with filters

### User Routes (Authentication Required)
- `GET /Account/Profile` - User dashboard
- `POST /Booking/Create` - Create new booking
- `POST /Review/Create` - Submit tour review

### Agency Routes (Agency Authentication Required)
- `GET /Agency/Dashboard` - Agency overview
- `POST /Tour/Create` - Create new tour
- `GET /Agency/Bookings` - Manage bookings
- `GET /Agency/Review` - View customer reviews

---

## 🤝 Contributing

1. **Fork the repository**
2. **Create a feature branch:** `git checkout -b feature/amazing-feature`
3. **Commit your changes:** `git commit -m 'Add amazing feature'`
4. **Push to the branch:** `git push origin feature/amazing-feature`
5. **Open a Pull Request**

### Development Guidelines
- Follow ASP.NET Core MVC conventions
- Use Entity Framework Core for data access
- Implement proper error handling and logging
- Write clean, documented code
- Test features thoroughly before submitting PRs

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🆘 Support

- **Live Demo:** [https://tripnest-r14u.onrender.com/](https://tripnest-r14u.onrender.com/)
- **Contact Form:** [Contact Support](https://tripnest-r14u.onrender.com/Home/ContactUs)
- **Issues:** [GitHub Issues](https://github.com/your-username/tripnest/issues)

---

## 👨‍💻 Authors

- **Manoj Shrestha** - Frontend Developer
  - [LinkedIn](https://www.linkedin.com/in/manoj-shrestha-43a64b177/)
  - [Instagram](https://www.instagram.com/manoj_sthaa)

- **Preeti Rajdhami** - Backend Developer
  - [LinkedIn](https://www.linkedin.com/in/preeti-rajdhami-103803244/)
  - [Instagram](https://www.instagram.com/pre.ettiii)

---

## 🙏 Acknowledgments

- Bootstrap team for the responsive framework
- Cloudinary for image management solution
- Render.com for hosting platform
- PostgreSQL community for the robust database
- ASP.NET Core team for the excellent framework

---

*Built with ❤️ using ASP.NET Core MVC*