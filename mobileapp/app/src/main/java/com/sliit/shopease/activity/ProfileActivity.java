package com.sliit.shopease.activity;

import android.content.Intent;
import android.os.Bundle;
import android.widget.Button;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.SharedPreferencesHelper;

public class ProfileActivity extends AppCompatActivity {
  Button prof_btn_logout;
  SharedPreferencesHelper sharedPreferencesHelper;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_profile_view);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
      Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
      v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
      return insets;
    });

    prof_btn_logout = findViewById(R.id.prof_btn_logout);
    prof_btn_logout.setOnClickListener(v->logout());

    sharedPreferencesHelper = new SharedPreferencesHelper(this);
  }

  void logout() {
    sharedPreferencesHelper.remove(PrefKeys.USER);

    //go to login page
    startActivity(new Intent(this, SignInActivity.class));
    finish();
  }
}