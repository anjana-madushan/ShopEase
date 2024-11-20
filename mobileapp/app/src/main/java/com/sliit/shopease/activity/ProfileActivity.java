package com.sliit.shopease.activity;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;

import androidx.activity.EdgeToEdge;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.sliit.shopease.R;
import com.sliit.shopease.constants.PrefKeys;
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.helpers.SharedPreferencesHelper;

public class ProfileActivity extends AppCompatActivity {
  SharedPreferencesHelper sharedPreferencesHelper;
  private EditText prof_edt_name;
  private EditText prof_edt_email;

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

    Button prof_btn_logout = findViewById(R.id.prof_btn_logout);
    prof_edt_email = findViewById(R.id.prof_edt_email);
    prof_edt_name = findViewById(R.id.prof_edt_name);
    Button prof_btn_update = findViewById(R.id.prof_btn_update);

    prof_btn_logout.setOnClickListener(v -> logout());
    prof_btn_update.setOnClickListener(this::update);

    sharedPreferencesHelper = new SharedPreferencesHelper(this);
  }

  void logout() {
    sharedPreferencesHelper.remove(PrefKeys.USER);

    //go to login page
    startActivity(new Intent(this, SignInActivity.class));
    finish();
  }

  void update(View v) {
    prof_edt_name.clearFocus();
    prof_edt_email.clearFocus();

    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    String name = prof_edt_name.getText().toString().trim();
    String email = prof_edt_email.getText().toString().trim();

    if(name.isEmpty() || email.isEmpty()){
      DialogHelper.showAlert(this, "Error", "Please enter both name and email");
      return;
    }

    DialogHelper.showLoading(this, "Updating...");
  }
}