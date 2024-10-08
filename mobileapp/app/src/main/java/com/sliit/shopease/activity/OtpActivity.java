package com.sliit.shopease.activity;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.KeyEvent;
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

public class OtpActivity extends AppCompatActivity {
  private final UserRepo userRepo = new UserRepo();

  String email;
  private EditText edt_otp1;
  private EditText edt_otp2;
  private EditText edt_otp3;
  private EditText edt_otp4;
  private EditText edt_otp5;
  private EditText edt_otp6;

  @Override
  protected void onCreate(Bundle savedInstanceState) {
    super.onCreate(savedInstanceState);
    EdgeToEdge.enable(this);
    setContentView(R.layout.activity_otp_screen);
    ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
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

    Button otp_btn_submit = findViewById(R.id.otp_btn_submit);

    edt_otp1 = findViewById(R.id.edt_otp1);
    edt_otp2 = findViewById(R.id.edt_otp2);
    edt_otp3 = findViewById(R.id.edt_otp3);
    edt_otp4 = findViewById(R.id.edt_otp4);
    edt_otp5 = findViewById(R.id.edt_otp5);
    edt_otp6 = findViewById(R.id.edt_otp6);

    otp_btn_submit.setOnClickListener(this::submit);

    setupOtpInputs();

    // get email from insets
    email = getIntent().getStringExtra("email");

    edt_otp1.requestFocus();
  }

  private void setupOtpInputs() {
    setOtpInputListener(edt_otp1, edt_otp2);
    setOtpInputListener(edt_otp2, edt_otp3);
    setOtpInputListener(edt_otp3, edt_otp4);
    setOtpInputListener(edt_otp4, edt_otp5);
    setOtpInputListener(edt_otp5, edt_otp6);
    setOtpInputListener(edt_otp6, null); // Last OTP input, no next field
  }

  private void setOtpInputListener(EditText currentEditText, EditText nextEditText) {
    currentEditText.addTextChangedListener(new TextWatcher() {
      @Override
      public void beforeTextChanged(CharSequence s, int start, int count, int after) {
      }

      @Override
      public void onTextChanged(CharSequence s, int start, int before, int count) {
        if (s.length() == 1) {
          // Move to next EditText when one digit is entered
          if (nextEditText != null) {
            nextEditText.requestFocus();
          }
        }
      }

      @Override
      public void afterTextChanged(Editable s) {
      }
    });

    // Handle backspace key to move to the previous EditText
    currentEditText.setOnKeyListener((v, keyCode, event) -> {
      if (keyCode == KeyEvent.KEYCODE_DEL && currentEditText.getText().length() == 0) {
        // Move to the previous EditText on backspace if the current field is empty
        if (currentEditText != edt_otp1) {
          previousEditText(currentEditText).requestFocus();
        }
      }
      return false;
    });
  }

  private EditText previousEditText(EditText currentEditText) {
    if (currentEditText == edt_otp2) return edt_otp1;
    if (currentEditText == edt_otp3) return edt_otp2;
    if (currentEditText == edt_otp4) return edt_otp3;
    if (currentEditText == edt_otp5) return edt_otp4;
    if (currentEditText == edt_otp6) return edt_otp5;
    return currentEditText;
  }

  private void submit(View v) {
    edt_otp1.clearFocus();
    edt_otp2.clearFocus();
    edt_otp3.clearFocus();
    edt_otp4.clearFocus();
    edt_otp5.clearFocus();
    edt_otp6.clearFocus();

    InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
    if (imm != null) {
      imm.hideSoftInputFromWindow(v.getWindowToken(), 0);
    }

    String otp1 = edt_otp1.getText().toString().trim();
    String otp2 = edt_otp2.getText().toString().trim();
    String otp3 = edt_otp3.getText().toString().trim();
    String otp4 = edt_otp4.getText().toString().trim();
    String otp5 = edt_otp5.getText().toString().trim();
    String otp6 = edt_otp6.getText().toString().trim();

    String otp = otp1 + otp2 + otp3 + otp4 + otp5 + otp6;

    //if otp.length != 6 show error
    if (otp.length() != 6) {
      DialogHelper.showAlert(this, "Error", "Please enter a valid OTP");
      return;
    }

    DialogHelper.showLoading(this, "Validating OTP...");

    userRepo.validateOtp(this, email, otp, new NetworkCallback<String>() {
      @Override
      public void onSuccess(String response) {
        DialogHelper.hideLoading();
        goToUpdatePassword();
      }

      @Override
      public void onFailure(ShopEaseError error) {
        String errorMsg = error.getMessage();
        if (error.getStatus() == 401) {
          errorMsg = "Incorrect OTP. Please Try again!";
        }
        DialogHelper.hideLoading();

        final String finalErrorMsg = errorMsg;
        runOnUiThread(() -> DialogHelper.showAlert(OtpActivity.this, "Error: ", finalErrorMsg));
      }
    });
  }

  private void goToUpdatePassword() {
    // add email to intent and go to update password activity
    Intent intent = new Intent(this, UpdatePasswordActivity.class);
    intent.putExtra("email", email);
    startActivity(intent);
    finish();
  }
}