// Import sample images that currently exist in the samples folder
import fitness3d from '../assets/samples/3d-fitness-tracker-21421754.png'
import abstractMeditation from '../assets/samples/abstract-meditation-d246b3d1.png'
import cartoonKids from '../assets/samples/cartoon-kids-learning-5e657009.png'
import clayCooking from '../assets/samples/clay-cooking-242e974c.png'
import flatShopping from '../assets/samples/flat-shopping-c503a610.png'
import flatElearning from '../assets/samples/flat-e-learning-4fd8b655.png'
import geometricAnalytics from '../assets/samples/geometric-analytics-86939866.png'
import glassmorphismCalendar from '../assets/samples/glassmorphism-calendar-48a2ed16.png'
import glassmorphismMessaging from '../assets/samples/glassmorphism-messaging-39a05052.png'
import gradientPhoto from '../assets/samples/gradient-photo-editor-90fa82e4.png'
import handDrawnJournal from '../assets/samples/hand-drawn-journal-09cef298.png'
import handDrawnNotes from '../assets/samples/hand-drawn-notes-58833bf9.png'
import isometricCity from '../assets/samples/isometric-city-builder-e77d501e.png'
import isometricDelivery from '../assets/samples/isometric-delivery-6172e83f.png'
import metallicTools from '../assets/samples/metallic-tools-aff85f6b.png'
import minimalWeather from '../assets/samples/minimal-weather-app-96254c75.png'
import minimalTask from '../assets/samples/minimal-task-manager-4b6b1673.png'
import neonMusic from '../assets/samples/neon-music-festival-903ac63e.png'
import pixelGaming from '../assets/samples/pixel-gaming-cf3bef66.png'
import pixelMusic from '../assets/samples/pixel-music-creator-69436e40.png'
import realisticCamera from '../assets/samples/realistic-camera-edb6b32e.png'
import realisticRealEstate from '../assets/samples/realistic-real-estate-ca3fdc1b.png'
import watercolorGarden from '../assets/samples/watercolor-garden-89eb5f63.png'

import type { IconStyle } from '../types'

export interface SampleImage {
  src: string
  alt: string
}

// Map styles to their sample images (only referencing files that exist)
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
    src: isometricCity,
    alt: 'City Builder - Isometric Style'
  },
  'Hand-drawn': {
    src: handDrawnJournal,
    alt: 'Journal - Hand-drawn Style'
  },
  'Geometric': {
    src: geometricAnalytics,
    alt: 'Analytics - Geometric Style'
  },
  'Abstract': {
    src: abstractMeditation,
    alt: 'Meditation - Abstract Style'
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

// Additional samples for gallery/showcase (only include existing files)
export const allSamples = {
  '3d': [fitness3d],
  abstract: [abstractMeditation],
  cartoon: [cartoonKids],
  clay: [clayCooking],
  flat: [flatShopping, flatElearning],
  geometric: [geometricAnalytics],
  glassmorphism: [glassmorphismCalendar, glassmorphismMessaging],
  gradient: [gradientPhoto],
  handDrawn: [handDrawnJournal, handDrawnNotes],
  isometric: [isometricCity, isometricDelivery],
  metallic: [metallicTools],
  minimal: [minimalWeather, minimalTask],
  neon: [neonMusic],
  pixel: [pixelGaming, pixelMusic],
  realistic: [realisticRealEstate, realisticCamera],
  watercolor: [watercolorGarden]
}
