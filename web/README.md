# IconGen AI - React Web Application

Beautiful, modern React web application for AI-powered icon generation.

## 🚀 Tech Stack

- **React 18** with TypeScript
- **Vite** - Lightning fast build tool
- **Tailwind CSS** - Utility-first styling
- **React Query** - Server state management
- **Zustand** - Client state management
- **React Hot Toast** - Beautiful notifications
- **Lucide React** - Icon library
- **React Colorful** - Color picker

## 📦 Features

- **Icon Generator** - Create stunning app icons with AI
- **18+ Styles** - Choose from 3D, Minimal, Gradient, Glassmorphism, and more
- **Custom Colors** - Full color customization with palettes
- **App Resources** - Generate complete asset packages for iOS, Android, Web
- **Beautiful UI** - Modern, responsive design with smooth animations
- **Real-time Preview** - See your icon as it's generated

## 🛠️ Development

### Prerequisites

- Node.js 20+
- npm or yarn

### Installation

```bash
# Install dependencies
npm install

# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

### Environment Variables

Create a `.env` file:

```env
VITE_API_ENDPOINT=http://localhost:7071/api
VITE_STRIPE_PUBLIC_KEY=pk_test_your_key
```

## 📁 Project Structure

```
web/
├── src/
│   ├── components/
│   │   ├── Layout/          # Header, Footer
│   │   ├── IconGenerator/   # Icon generation UI
│   │   ├── AppResources/    # App resources page
│   │   ├── Dashboard/       # User dashboard
│   │   └── Pricing/         # Pricing page
│   ├── services/
│   │   └── api.ts           # API client
│   ├── types/
│   │   └── index.ts         # TypeScript types
│   ├── lib/
│   │   └── utils.ts         # Utility functions
│   ├── App.tsx              # Main app component
│   └── main.tsx             # Entry point
├── public/                  # Static assets
└── index.html              # HTML template
```

## 🎨 Design System

### Colors

- **Primary**: Blue 600 (#2563EB)
- **Secondary**: Purple 600 (#9333EA)
- **Accent**: Pink 600 (#DB2777)

### Typography

- **Headings**: Inter font, bold weights
- **Body**: Inter font, regular weights

### Components

- Rounded corners: 0.5rem (default)
- Shadows: Tailwind shadow utilities
- Animations: Smooth transitions, hover effects

## 🚀 Deployment

### Azure Static Web Apps

Automatically deploys via GitHub Actions:

1. Push to `main` branch
2. GitHub Actions builds and deploys
3. Live at your Azure Static Web Apps URL

### GitHub Secrets Required

- `AZURE_STATIC_WEB_APPS_API_TOKEN` - From Azure Portal
- `AZURE_FUNCTIONS_URL` - Your Azure Functions endpoint
- `STRIPE_PUBLIC_KEY` - Stripe publishable key

## 📝 Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## 🤝 API Integration

The app connects to Azure Functions backend:

### Endpoints

- `POST /api/generate-icon` - Generate new icon
- `POST /api/enhance-prompt` - Enhance user prompt
- `POST /api/generate-app-resources` - Generate app resources
- `GET /api/user` - Get user info
- `GET /api/user/icons` - Get user's icons

## 📄 License

MIT License - See LICENSE file for details

## 🎯 Next Steps

- [ ] Add user authentication
- [ ] Implement payment integration
- [ ] Add icon gallery
- [ ] Add batch generation
- [ ] Mobile app version
