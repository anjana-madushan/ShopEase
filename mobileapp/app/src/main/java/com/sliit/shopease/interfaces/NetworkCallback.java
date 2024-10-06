package com.sliit.shopease.interfaces;

public interface NetworkCallback {
  void onSuccess(String response);

  void onFailure(String error);
}
