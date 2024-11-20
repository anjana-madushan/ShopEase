package com.sliit.shopease.models;

import androidx.annotation.NonNull;

import java.net.SocketTimeoutException;
import java.net.URISyntaxException;

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
    if(exception != null){
      String message = exception.getMessage();

      //if exception is type timeout, give user friendly message
      if(exception instanceof SocketTimeoutException){
        message = "Connection timed out";
      }else if(exception instanceof URISyntaxException){
        message = "Please set Base URL";
      }

      return message;
    }

    String errorMsg = message;

    if(status == 409){
      errorMsg = "User already exists";
    }else if(status == 404){
      errorMsg = "User not found";
    }
    return errorMsg;
  }

  public Response getResponse() {
    return response;
  }

  public Exception getException(){
    return exception;
  }
}
