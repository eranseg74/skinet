using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class CouponService : ICouponService
{
  public CouponService(IConfiguration config)
  {
    StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
  }
  public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
  {
    var promotionService = new PromotionCodeService();
    var couponService = new Stripe.CouponService();
    var options = new PromotionCodeListOptions
    {
      Code = code
    };
    var promotionCodes = await promotionService.ListAsync(options);
    var promotionCode = promotionCodes.Data.FirstOrDefault();
    if (promotionCode != null)
    {
      var coupon = await couponService.GetAsync(promotionCode.Promotion.CouponId);
      if (coupon != null)
      {
        return new AppCoupon
        {
          CouponId = coupon.Id,
          AmountOff = coupon.AmountOff,
          PercentOff = coupon.PercentOff,
          Name = coupon.Name,
          PromotionCode = promotionCode.Code
        };
      }
    }
    return null;
  }
}
