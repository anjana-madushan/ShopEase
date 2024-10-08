package com.sliit.shopease.repository;

import android.content.Context;

import com.sliit.shopease.constants.ApiEndPoints;
import com.sliit.shopease.helpers.NetworkHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;

public class AuthenticationRepo {
  final NetworkHelper networkHelper = NetworkHelper.getInstance();

  public void sendOtp(Context context, String email, NetworkCallback callback) {
    networkHelper.post(context, ApiEndPoints.SEND_OTP + "/" + email + "/customer", null, new NetworkCallback() {
      @Override
      public void onSuccess(String response) {
        callback.onSuccess("success");
      }

      @Override
      public void onFailure(ShopEaseError error) {
        callback.onFailure(error);
      }
    });
  }
}
