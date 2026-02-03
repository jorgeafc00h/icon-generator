// Import sample images that currently exist in the samples folder
import fitness3d from '../assets/samples/resized/512/3d-fitness-tracker-21421754.png'
import abstractMeditation from '../assets/samples/resized/512/abstract-meditation-d246b3d1.png'
import cartoonKids from '../assets/samples/resized/512/cartoon-kids-learning-5e657009.png'
import clayCooking from '../assets/samples/resized/512/clay-cooking-242e974c.png'
import flatShopping from '../assets/samples/resized/512/flat-shopping-c503a610.png'
import flatElearning from '../assets/samples/resized/512/flat-e-learning-4fd8b655.png'
import geometricAnalytics from '../assets/samples/resized/512/geometric-analytics-86939866.png'
import glassmorphismCalendar from '../assets/samples/resized/512/glassmorphism-calendar-48a2ed16.png'
import glassmorphismMessaging from '../assets/samples/resized/512/glassmorphism-messaging-39a05052.png'
import gradientPhoto from '../assets/samples/resized/512/gradient-photo-editor-90fa82e4.png'
import handDrawnNotes from '../assets/samples/resized/512/hand-drawn-notes-58833bf9.png'
import isometricCity from '../assets/samples/resized/512/isometric-city-builder-e77d501e.png'
import isometricDeliveryB from '../assets/samples/resized/512/isometric-delivery-6172e83f.png'
import metallicTools from '../assets/samples/resized/512/metallic-tools-aff85f6b.png'
import minimalTask from '../assets/samples/resized/512/minimal-task-manager-7cb9c19f.png'
import minimalWeather from '../assets/samples/resized/512/minimal-weather-app-d82e5a5f.png'
import neonMusic from '../assets/samples/resized/512/neon-music-festival-903ac63e.png'
import pixelGaming from '../assets/samples/resized/512/pixel-gaming-cf3bef66.png'
import pixelMusic from '../assets/samples/resized/512/pixel-music-creator-69436e40.png'
import realisticCamera from '../assets/samples/resized/512/realistic-camera-edb6b32e.png'
import realisticRealEstate from '../assets/samples/resized/512/realistic-real-estate-ca3fdc1b.png'
import watercolorGarden from '../assets/samples/resized/512/watercolor-garden-89eb5f63.png'

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
    src: handDrawnNotes,
    alt: 'Notes - Hand-drawn Style'
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
  'Retro': {
    src: pixelGaming,
    alt: 'Arcade - Retro Style'
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
  handDrawn: [handDrawnNotes],
  isometric: [isometricCity, isometricDeliveryB],
  metallic: [metallicTools],
  minimal: [minimalWeather, minimalTask],
  neon: [neonMusic],
  pixel: [pixelGaming, pixelMusic],
  realistic: [realisticRealEstate, realisticCamera],
  watercolor: [watercolorGarden]
}
