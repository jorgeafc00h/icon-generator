// Import all sample images
import fitness3d from '../assets/samples/3d-fitness-tracker-2677e664.png'
import abstractMeditation from '../assets/samples/abstract-meditation-d246b3d1.png'
import abstractArt from '../assets/samples/abstract-art-gallery-26293762.png'
import cartoonKids from '../assets/samples/cartoon-kids-learning-5e657009.png'
import cartoonPet from '../assets/samples/cartoon-pet-care-51f9c437.png'
import clayCooking from '../assets/samples/clay-cooking-57cf4733.png'
import clayTravel from '../assets/samples/clay-travel-4e770059.png'
import flatShopping from '../assets/samples/flat-shopping-c503a610.png'
import flatElearning from '../assets/samples/flat-e-learning-eca381c7.png'
import geometricAnalytics from '../assets/samples/geometric-analytics-86939866.png'
import geometricCrypto from '../assets/samples/geometric-crypto-4c230d95.png'
import glassmorphismCalendar from '../assets/samples/glassmorphism-calendar-80b983f9.png'
import glassmorphismMessaging from '../assets/samples/glassmorphism-messaging-39a05052.png'
import gradientPhoto from '../assets/samples/gradient-photo-editor-b3f9ed48.png'
import handDrawnJournal from '../assets/samples/hand-drawn-journal-a4cb6ed3.png'
import handDrawnNotes from '../assets/samples/hand-drawn-notes-ec7a7189.png'
import isometricCity from '../assets/samples/isometric-city-builder-e77d501e.png'
import isometricDelivery from '../assets/samples/isometric-delivery-6172e83f.png'
import metallicLuxury from '../assets/samples/metallic-luxury-88882e12.png'
import metallicTools from '../assets/samples/metallic-tools-aff85f6b.png'
import minimalWeather from '../assets/samples/minimal-weather-app-96254c75.png'
import minimalTask from '../assets/samples/minimal-task-manager-4b6b1673.png'
import neonMusic from '../assets/samples/neon-music-festival-903ac63e.png'
import pixelGaming from '../assets/samples/pixel-gaming-cf3bef66.png'
import pixelMusic from '../assets/samples/pixel-music-creator-3d2a1dec.png'
import realisticCamera from '../assets/samples/realistic-camera-edb6b32e.png'
import realisticRealEstate from '../assets/samples/realistic-real-estate-ca3fdc1b.png'
import retroArcade from '../assets/samples/retro-arcade-8eeae802.png'
import watercolorGarden from '../assets/samples/watercolor-garden-89eb5f63.png'
import watercolorPainting from '../assets/samples/watercolor-painting-05b0a373.png'

import type { IconStyle } from '../types'

export interface SampleImage {
  src: string
  alt: string
}

// Map styles to their sample images
export const styleSamples: Record<IconStyle, SampleImage> = {
  '3D': {
    src: fitness3d,
    alt: 'Fitness Tracker - 3D Style'
  },
  'Minimal': {
    src: minimalWeather,
    alt: 'Weather App - Minimal Style'
  },
  'Gradient': {
    src: gradientPhoto,
    alt: 'Photo Editor - Gradient Style'
  },
  'Glassmorphism': {
    src: glassmorphismCalendar,
    alt: 'Calendar - Glassmorphism Style'
  },
  'Neomorphism': {
    src: minimalTask,
    alt: 'Task Manager - Neomorphism Style'
  },
  'Clay': {
    src: clayCooking,
    alt: 'Cooking App - Clay Style'
  },
  'Pixel': {
    src: pixelGaming,
    alt: 'Gaming - Pixel Art Style'
  },
  'Flat': {
    src: flatShopping,
    alt: 'Shopping - Flat Design Style'
  },
  'Isometric': {
    src: isometricDelivery,
    alt: 'Delivery - Isometric Style'
  },
  'Hand-drawn': {
    src: handDrawnJournal,
    alt: 'Journal - Hand-drawn Style'
  },
  'Geometric': {
    src: geometricCrypto,
    alt: 'Crypto - Geometric Style'
  },
  'Abstract': {
    src: abstractMeditation,
    alt: 'Meditation - Abstract Style'
  },
  'Retro': {
    src: retroArcade,
    alt: 'Arcade - Retro Style'
  },
  'Neon': {
    src: neonMusic,
    alt: 'Music Festival - Neon Style'
  },
  'Watercolor': {
    src: watercolorGarden,
    alt: 'Garden - Watercolor Style'
  },
  'Metallic': {
    src: metallicTools,
    alt: 'Tools - Metallic Style'
  },
  'Cartoon': {
    src: cartoonKids,
    alt: 'Kids Learning - Cartoon Style'
  },
  'Realistic': {
    src: realisticCamera,
    alt: 'Camera - Realistic Style'
  }
}

// Additional samples for gallery/showcase
export const allSamples = {
  '3d': [fitness3d],
  abstract: [abstractMeditation, abstractArt],
  cartoon: [cartoonKids, cartoonPet],
  clay: [clayCooking, clayTravel],
  flat: [flatShopping, flatElearning],
  geometric: [geometricAnalytics, geometricCrypto],
  glassmorphism: [glassmorphismCalendar, glassmorphismMessaging],
  gradient: [gradientPhoto],
  handDrawn: [handDrawnJournal, handDrawnNotes],
  isometric: [isometricCity, isometricDelivery],
  metallic: [metallicLuxury, metallicTools],
  minimal: [minimalWeather, minimalTask],
  neon: [neonMusic],
  pixel: [pixelGaming, pixelMusic],
  realistic: [realisticRealEstate],
  retro: [retroArcade],
  watercolor: [watercolorGarden, watercolorPainting]
}
