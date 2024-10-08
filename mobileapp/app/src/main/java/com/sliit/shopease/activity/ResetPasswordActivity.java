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
import com.sliit.shopease.helpers.DialogHelper;
import com.sliit.shopease.interfaces.NetworkCallback;
import com.sliit.shopease.models.ShopEaseError;
import com.sliit.shopease.repository.UserRepo;

public class ResetPasswordActivity extends AppCompatActivity {
  private final UserRepo userRepo = new UserRepo();
  private EditText reset_edt_email;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_reset_password);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.activity_resetPassword), (v, insets) -> {
      Insets systemBarsInsets = insets.getInsets(WindowInsetsCompat.Type.systemBars());

      // Apply system bar insets (status and navigation bar)
      v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, 0);

      // Handle the keyboard (IME) inset
      Insets imeInsetsType = insets.getInsets(WindowInsetsCompat.Type.ime());
      if (insets.isVisible(WindowInsetsCompat.Type.ime())) {
        // Keyboard is visible, add padding to avoid overlapping
        v.setPadding(systemBarsInsets.left, systemBarsInsets.top, systemBarsInsets.right, imeInsetsType.bottom);
      }

      return insets;
    });

    Button reset_btn_submit = findViewById(R.id.reset_btn_submit);

    reset_edt_email = findViewById(R.id.reset_edt_email);

    reset_btn_submit.setOnClickListener(this::submit);
  }

  void submit(View v) {
    reset_edt_email.clearFocus();

    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    String email = reset_edt_email.getText().toString();

    if (email.isEmpty()) {
      DialogHelper.showAlert(this, "Error", "Please enter email");
      return;
    }

    DialogHelper.showLoading(this, "Sending OTP...");

    userRepo.sendOtp(this, email, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {
        System.out.println("Reset password:" + response);
        DialogHelper.hideLoading();

        //goto otp screen
        Intent intent = new Intent(ResetPasswordActivity.this, OtpActivity.class);
        intent.putExtra("email", email);
        startActivity(intent);
        finish();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        System.out.println(error);
        DialogHelper.hideLoading();
        runOnUiThread(() -> DialogHelper.showAlert(ResetPasswordActivity.this, "Error: ", error.getMessage()));
      }
    });
  }
}