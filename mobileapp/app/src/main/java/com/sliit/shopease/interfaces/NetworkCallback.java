package com.sliit.shopease.interfaces;

import com.sliit.shopease.models.ShopEaseError;

public interface NetworkCallback {
  void onSuccess(String response);

  void onFailure(ShopEaseError error);
}
