import { Check, Zap, Star, Rocket } from 'lucide-react'

interface PricingProps {
  onNavigate?: (page: any) => void
}

const plans = [
  {
    name: 'Starter',
    credits: 10,
    bonusCredits: 0,
    price: 12,
    icon: Zap,
    features: ['10 icon generations', 'All 18+ styles', 'Standard quality', 'Commercial license'],
  },
  {
    name: 'Pro',
    credits: 50,
    bonusCredits: 10,
    price: 29,
    icon: Star,
    popular: true,
    features: ['60 total credits (50 + 10 bonus)', 'All 18+ styles', 'HD quality included', 'Commercial license', 'Priority support'],
  },
  {
    name: 'Business',
    credits: 150,
    bonusCredits: 15,
    price: 49,
    icon: Rocket,
    features: ['165 total credits (150 + 15 bonus)', 'All 18+ styles', 'HD quality included', 'Commercial license', 'Priority support', 'Bulk generation'],
  },
]

export function Pricing({ onNavigate }: PricingProps) {
  const handleGetStarted = () => {
    // Check if user is logged in
    const accessToken = localStorage.getItem('accessToken')
    if (!accessToken) {
      // Redirect to profile to sign in
      if (onNavigate) {
        onNavigate('profile')
      }
    } else {
      // Already logged in, go to pricing/purchase
      if (onNavigate) {
        onNavigate('profile') // Profile has the purchase modal
      }
    }
  }
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="text-center mb-12">
        <h1 className="text-4xl md:text-5xl font-bold mb-4">Simple, Transparent Pricing</h1>
        <p className="text-xl text-gray-600">Choose the plan that fits your needs</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {plans.map((plan) => {
          const Icon = plan.icon
          return (
            <div
              key={plan.name}
              className={`bg-white rounded-2xl p-8 border-2 ${
                plan.popular ? 'border-blue-500 shadow-xl scale-105' : 'border-gray-200'
              } relative`}
            >
              {plan.popular && (
                <div className="absolute -top-4 left-1/2 transform -translate-x-1/2 bg-gradient-to-r from-blue-600 to-purple-600 text-white px-4 py-1 rounded-full text-sm font-medium">
                  Most Popular
                </div>
              )}

              <div className={`w-12 h-12 bg-${plan.popular ? 'blue' : 'gray'}-100 rounded-lg flex items-center justify-center mb-4`}>
                <Icon className={`w-6 h-6 text-${plan.popular ? 'blue' : 'gray'}-600`} />
              </div>

              <h3 className="text-2xl font-bold mb-2">{plan.name}</h3>
              <div className="mb-6">
                <span className="text-4xl font-bold">${plan.price}</span>
                <div className="text-gray-600 mt-1">
                  {plan.bonusCredits > 0 ? (
                    <>
                      <span className="text-2xl font-bold text-gray-900">{plan.credits + plan.bonusCredits}</span> credits
                      <div className="text-sm text-green-600 font-semibold mt-1">
                        🎁 Includes {plan.bonusCredits} bonus credits
                      </div>
                    </>
                  ) : (
                    <span>{plan.credits} credits</span>
                  )}
                </div>
              </div>

              <ul className="space-y-3 mb-8">
                {plan.features.map((feature) => (
                  <li key={feature} className="flex items-start gap-2">
                    <Check className="w-5 h-5 text-green-500 flex-shrink-0 mt-0.5" />
                    <span className="text-gray-600">{feature}</span>
                  </li>
                ))}
              </ul>

              <button
                onClick={handleGetStarted}
                className={`w-full py-3 rounded-lg font-semibold transition-all ${
                  plan.popular
                    ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white hover:shadow-lg'
                    : 'bg-gray-100 text-gray-900 hover:bg-gray-200'
                }`}
              >
                Get Started
              </button>
            </div>
          )
        })}
      </div>

      <div className="mt-12 text-center text-gray-600">
        <p>All plans include commercial license and full ownership of generated icons</p>
      </div>
    </div>
  )
}
