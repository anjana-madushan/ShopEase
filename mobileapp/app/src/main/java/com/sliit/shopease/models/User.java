package com.sliit.shopease.models;

import com.google.gson.Gson;

public class User {
  private final String id;
  private final String username;
  private final String email;
  private final String token;

  public User(String id, String username, String email, String token) {
    this.id = id;
    this.username = username;
    this.email = email;
    this.token = token;
  }

  public String getId() {
    return id;
  }

  public String getUsername() {
    return username;
  }

  public String getEmail() {
    return email;
  }

  public String getToken() {
    return token;
  }

  // toJson method
  public String toJson() {
    Gson gson = new Gson();
    return gson.toJson(this);
  }

  // fromJson method
  public static User fromJson(String jsonString) {
    Gson gson = new Gson();
    return gson.fromJson(jsonString, User.class);
  }
}
