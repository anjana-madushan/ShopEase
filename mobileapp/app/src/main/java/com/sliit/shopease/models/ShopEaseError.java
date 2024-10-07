package com.sliit.shopease.models;

import androidx.annotation.NonNull;

import okhttp3.Response;

public class ShopEaseError {
  final Integer status;
  final String message;
  final Response response;
  final Exception exception;

  public ShopEaseError(int status, String message, Response response) {
    this.status = status;
    this.message = message;
    this.response = response;
    this.exception = null;
  }

  public ShopEaseError(Exception exception) {
    this.status = null;
    this.message = exception.getMessage();
    this.response = null;
    this.exception = exception;
  }

  public ShopEaseError() {
    this.status = null;
    this.message = "received Null";
    this.response = null;
    this.exception = null;
  }

  public Integer getStatus() {
    return status;
  }

  public String getMessage() {
    return message;
  }

  public Response getResponse() {
    return response;
  }
}
