package com.sliit.shopease.interfaces;

import com.sliit.shopease.models.ShopEaseError;

public interface NetworkCallback<T> {
  void onSuccess(T response);

  void onFailure(ShopEaseError error);
}
