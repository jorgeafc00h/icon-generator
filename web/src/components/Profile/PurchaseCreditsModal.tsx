import { useState } from 'react'
import type { CreditPackage } from '../../types'
import { X, Zap, Star, Crown, Check, CreditCard } from 'lucide-react'

interface PurchaseCreditsModalProps {
  onClose: () => void
  currentCredits: number
}

const creditPackages: CreditPackage[] = [
  {
    id: 'starter',
    name: 'Starter',
    credits: 50,
    priceInCents: 499, // $4.99
  },
  {
    id: 'popular',
    name: 'Popular',
    credits: 150,
    priceInCents: 999, // $9.99
    popular: true,
  },
  {
    id: 'pro',
    name: 'Professional',
    credits: 500,
    priceInCents: 2999, // $29.99
  },
  {
    id: 'business',
    name: 'Business',
    credits: 1500,
    priceInCents: 7999, // $79.99
  },
]

export function PurchaseCreditsModal({ onClose, currentCredits }: PurchaseCreditsModalProps) {
  const [selectedPackage, setSelectedPackage] = useState<CreditPackage | null>(
    creditPackages.find(p => p.popular) || null
  )
  const [loading, setLoading] = useState(false)

  const handlePurchase = async () => {
    if (!selectedPackage) return

    const accessToken = localStorage.getItem('accessToken')
    const userId = localStorage.getItem('userId')

    if (!accessToken || !userId) {
      alert('Please sign in to purchase credits')
      return
    }

    setLoading(true)
    try {
      const response = await fetch(`${import.meta.env.VITE_API_ENDPOINT}/payments/checkout`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        body: JSON.stringify({
          userId: userId,
          packageId: selectedPackage.id,
          successUrl: `${window.location.origin}/profile?payment=success`,
          cancelUrl: `${window.location.origin}/profile?payment=canceled`
        })
      })

      if (!response.ok) {
        const error = await response.json()
        throw new Error(error.message || 'Failed to create checkout session')
      }

      const { checkoutUrl } = await response.json()

      // Redirect to Stripe Checkout
      window.location.href = checkoutUrl
    } catch (error) {
      console.error('Error purchasing credits:', error)
      alert(error instanceof Error ? error.message : 'Failed to initiate purchase. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  const getPackageIcon = (packageId: string) => {
    switch (packageId) {
      case 'starter':
        return Zap
      case 'popular':
        return Star
      case 'pro':
        return Crown
      case 'business':
        return Crown
      default:
        return Zap
    }
  }

  const getPackageColor = (packageId: string) => {
    switch (packageId) {
      case 'starter':
        return 'from-blue-500 to-blue-600'
      case 'popular':
        return 'from-purple-500 to-purple-600'
      case 'pro':
        return 'from-yellow-500 to-orange-600'
      case 'business':
        return 'from-pink-500 to-red-600'
      default:
        return 'from-blue-500 to-blue-600'
    }
  }

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 flex justify-between items-center z-10 rounded-t-2xl">
          <div>
            <h2 className="text-2xl font-bold">Buy Credits</h2>
            <p className="text-gray-600 mt-1">
              You have <span className="font-semibold text-blue-600">{currentCredits} credits</span>
            </p>
          </div>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <X size={24} />
          </button>
        </div>

        {/* Pricing Cards */}
        <div className="p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
            {creditPackages.map(pkg => {
              const Icon = getPackageIcon(pkg.id)
              const isSelected = selectedPackage?.id === pkg.id
              const pricePerCredit = (pkg.priceInCents / 100 / pkg.credits).toFixed(3)

              return (
                <button
                  key={pkg.id}
                  onClick={() => setSelectedPackage(pkg)}
                  className={`
                    relative p-6 rounded-xl border-2 transition-all text-left
                    ${isSelected
                      ? 'border-blue-600 bg-blue-50 shadow-lg scale-105'
                      : 'border-gray-200 hover:border-blue-300 hover:shadow-md'}
                  `}
                >
                  {pkg.popular && (
                    <div className="absolute -top-3 left-1/2 -translate-x-1/2">
                      <div className="bg-gradient-to-r from-purple-600 to-pink-600 text-white text-xs font-bold px-3 py-1 rounded-full">
                        MOST POPULAR
                      </div>
                    </div>
                  )}

                  {isSelected && (
                    <div className="absolute top-4 right-4">
                      <div className="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center">
                        <Check size={14} className="text-white" />
                      </div>
                    </div>
                  )}

                  <div className={`w-12 h-12 bg-gradient-to-br ${getPackageColor(pkg.id)} rounded-lg flex items-center justify-center mb-4`}>
                    <Icon size={24} className="text-white" />
                  </div>

                  <h3 className="font-bold text-lg mb-2">{pkg.name}</h3>

                  <div className="mb-3">
                    <div className="text-3xl font-bold text-gray-900">
                      {pkg.credits}
                    </div>
                    <div className="text-sm text-gray-600">credits</div>
                  </div>

                  <div className="mb-4">
                    <div className="text-2xl font-bold text-gray-900">
                      ${(pkg.priceInCents / 100).toFixed(2)}
                    </div>
                    <div className="text-xs text-gray-500">
                      ${pricePerCredit}/credit
                    </div>
                  </div>

                  {pkg.id === 'popular' && (
                    <div className="mt-4 pt-4 border-t border-gray-200">
                      <div className="text-xs font-medium text-purple-600">
                        Best Value • Save 33%
                      </div>
                    </div>
                  )}
                </button>
              )
            })}
          </div>

          {/* What you get */}
          <div className="bg-gray-50 rounded-xl p-6 mb-6">
            <h3 className="font-semibold text-gray-900 mb-4">What can you create?</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="flex items-start gap-3">
                <div className="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Zap size={16} className="text-blue-600" />
                </div>
                <div>
                  <div className="font-medium text-gray-900">App Icons</div>
                  <div className="text-sm text-gray-600">
                    1 credit = 1 app icon (18+ styles)
                  </div>
                </div>
              </div>

              <div className="flex items-start gap-3">
                <div className="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Star size={16} className="text-purple-600" />
                </div>
                <div>
                  <div className="font-medium text-gray-900">Screen Mockups</div>
                  <div className="text-sm text-gray-600">
                    1 credit = 1 screen mockup (HD quality)
                  </div>
                </div>
              </div>

              <div className="flex items-start gap-3">
                <div className="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Check size={16} className="text-green-600" />
                </div>
                <div>
                  <div className="font-medium text-gray-900">Platform Assets</div>
                  <div className="text-sm text-gray-600">
                    iOS, Android, Web, macOS resources
                  </div>
                </div>
              </div>

              <div className="flex items-start gap-3">
                <div className="w-8 h-8 bg-yellow-100 rounded-lg flex items-center justify-center flex-shrink-0">
                  <Crown size={16} className="text-yellow-600" />
                </div>
                <div>
                  <div className="font-medium text-gray-900">HD Quality</div>
                  <div className="text-sm text-gray-600">
                    1024x1024px high-resolution output
                  </div>
                </div>
              </div>
            </div>
          </div>

          {/* Purchase Button */}
          {selectedPackage && (
            <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl p-6 text-white">
              <div className="flex flex-col md:flex-row items-center justify-between gap-4">
                <div>
                  <div className="text-sm opacity-90 mb-1">Selected Package</div>
                  <div className="text-2xl font-bold">
                    {selectedPackage.name} - {selectedPackage.credits} Credits
                  </div>
                  <div className="text-sm opacity-90 mt-1">
                    ${(selectedPackage.priceInCents / 100).toFixed(2)} one-time payment
                  </div>
                </div>
                <button
                  onClick={handlePurchase}
                  disabled={loading}
                  className="bg-white text-blue-600 px-8 py-4 rounded-lg font-bold text-lg hover:bg-blue-50 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                >
                  {loading ? (
                    <>
                      <div className="animate-spin rounded-full h-5 w-5 border-2 border-blue-600 border-t-transparent" />
                      Processing...
                    </>
                  ) : (
                    <>
                      <CreditCard size={20} />
                      Purchase Now
                    </>
                  )}
                </button>
              </div>
            </div>
          )}

          {/* Security Note */}
          <div className="mt-6 text-center text-sm text-gray-500">
            <p>Secure checkout powered by Stripe • Your payment information is encrypted</p>
          </div>
        </div>
      </div>
    </div>
  )
}
