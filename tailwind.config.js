/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    './Components/**/*.{razor,html,cshtml,cs}',
    './Layout/**/*.{razor,html,cshtml,cs}',
    './Pages/**/*.{razor,html,cshtml,cs}',
    './Services/**/*.{razor,html,cshtml,cs}',
    './Models/**/*.{razor,html,cshtml,cs}',
    './App.razor',
    './_Imports.razor',
    './wwwroot/index.html'
  ],
  theme: {
    extend: {
      colors: {
        slate: {
          750: '#233044',
          850: '#172033',
        },
        brand: {
          50: 'rgb(var(--color-brand-50) / <alpha-value>)',
          100: 'rgb(var(--color-brand-100) / <alpha-value>)',
          200: 'rgb(var(--color-brand-200) / <alpha-value>)',
          300: 'rgb(var(--color-brand-300) / <alpha-value>)',
          400: 'rgb(var(--color-brand-400) / <alpha-value>)',
          500: 'rgb(var(--color-brand-500) / <alpha-value>)',
          600: 'rgb(var(--color-brand-600) / <alpha-value>)',
          700: 'rgb(var(--color-brand-700) / <alpha-value>)',
        },
        surface: {
          DEFAULT: 'rgb(var(--color-surface) / <alpha-value>)',
          muted: 'rgb(var(--color-surface-muted) / <alpha-value>)',
          elevated: 'rgb(var(--color-surface-elevated) / <alpha-value>)',
        }
      },
      screens: {
        'xs': '475px',
      },
      keyframes: {
        fadeIn: {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
        fadeInUp: {
          '0%': { opacity: '0', transform: 'translateY(20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        fadeInDown: {
          '0%': { opacity: '0', transform: 'translateY(-20px)' },
          '100%': { opacity: '1', transform: 'translateY(0)' },
        },
        fadeInLeft: {
          '0%': { opacity: '0', transform: 'translateX(-20px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' },
        },
        fadeInRight: {
          '0%': { opacity: '0', transform: 'translateX(20px)' },
          '100%': { opacity: '1', transform: 'translateX(0)' },
        },
        zoomIn: {
          '0%': { opacity: '0', transform: 'scale(0.9)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        zoomOut: {
          '0%': { opacity: '0', transform: 'scale(1.1)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        bounceIn: {
          '0%': { opacity: '0', transform: 'scale(0.3)' },
          '50%': { opacity: '0.9', transform: 'scale(1.05)' },
          '70%': { transform: 'scale(0.95)' },
          '100%': { opacity: '1', transform: 'scale(1)' },
        },
        shimmer: {
          '0%': { backgroundPosition: '-200% 0' },
          '100%': { backgroundPosition: '200% 0' },
        },
        pulseGlow: {
          '0%, 100%': { opacity: '1', filter: 'drop-shadow(0 0 8px rgba(99, 102, 241, 0.6))' },
          '50%': { opacity: '0.7', filter: 'drop-shadow(0 0 2px rgba(99, 102, 241, 0.2))' },
        },
        marquee: {
          '0%': { transform: 'translateX(0%)' },
          '100%': { transform: 'translateX(-100%)' },
        },
        marqueeReverse: {
          '0%': { transform: 'translateX(-100%)' },
          '100%': { transform: 'translateX(0%)' },
        }
      },
      animation: {
        'fade-in': 'fadeIn 0.4s ease-out forwards',
        'fade-in-up': 'fadeInUp 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'fade-in-down': 'fadeInDown 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'fade-in-left': 'fadeInLeft 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'fade-in-right': 'fadeInRight 0.5s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'zoom-in': 'zoomIn 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'zoom-out': 'zoomOut 0.4s cubic-bezier(0.16, 1, 0.3, 1) forwards',
        'bounce-in': 'bounceIn 0.6s cubic-bezier(0.34, 1.56, 0.64, 1) forwards',
        'shimmer': 'shimmer 2s infinite linear',
        'pulse-glow': 'pulseGlow 2s infinite ease-in-out',
        'marquee': 'marquee var(--marquee-duration, 25s) linear infinite',
        'marquee-reverse': 'marqueeReverse var(--marquee-duration, 25s) linear infinite',
      }
    },
  },
  plugins: [],
}
