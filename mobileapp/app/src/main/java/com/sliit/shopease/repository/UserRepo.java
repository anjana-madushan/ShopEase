package com.sliit.shopease.repository;

import android.content.Context;

import com.sliit.shopease.constants.ApiEndPoints;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.NetworkHelper;
import com.sliit.shopease.helpers.SharedPreferencesHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.models.User;

import java.util.HashMap;
import java.util.Map;

public class UserRepo {
  final NetworkHelper networkHelper = NetworkHelper.getInstance();

  public void sendOtp(Context context, String email, NetworkCallback<String> callback) {
    networkHelper.post(context, ApiEndPoints.SEND_OTP + email + "/customer", null, new NetworkCallback<String>() {
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

  public void updateUser(Context context, String email, String username, NetworkCallback<String> callback) {
    SharedPreferencesHelper sharedPreferencesHelper = new SharedPreferencesHelper(context);
    User user = User.fromJson(sharedPreferencesHelper.getString(PrefKeys.USER, ""));


    networkHelper.put(context, ApiEndPoints.UPDATE_USER + user.getId(), null, new NetworkCallback<String>() {
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

  public void validateOtp(Context context, String email, String otp, NetworkCallback<String> callback) {
    final Map<String, String> jsonBody = new HashMap<>();
    jsonBody.put("Code", otp);

    networkHelper.post(context, ApiEndPoints.VALIDATE_OTP + email + "/customer", jsonBody, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {
        System.out.println(response);
        callback.onSuccess("success");
      }

      @Override
      public void onFailure(ShopEaseError error) {
        System.out.println(error);
        callback.onFailure(error);
      }
    });
  }

  public void updatePassword(Context context, String password, NetworkCallback callback) {

  }
}
