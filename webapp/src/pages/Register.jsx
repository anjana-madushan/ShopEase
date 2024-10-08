import React from 'react';
import '../styles/login.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import loginImage from '../assets/images/reg-background.jpg'; 


const Register = () => {
    return (
        <div className="container-fluid">
            
                <div className="login-box d-flex">
                    <div className="login-form">
                        <p className="text-primary fw-bold ">Shop Easy</p>
                        <div className="d-flex justify-content-center align-items-center">
                            <h1>Welcome</h1>
                        </div>
                        <p className="text-center">Please enter your details to register</p>
                        <form>
                            <div className="mb-3">
                                <input type="name" className="form-control" placeholder="first name" />
                            </div>
                            <div className="mb-3">
                                <input type="name" className="form-control" placeholder="last name" />
                            </div>
                            <div className="mb-3">
                                <input type="email" className="form-control" placeholder="email" />
                            </div>
                            {/* <div className="d-flex justify-content-between align-items-center">
                                <div className="form-check">
                                    <input type="checkbox" className="form-check-input" id="rememberMe" />
                                    <label className="form-check-label" htmlFor="rememberMe">Remember me</label>
                                </div>
                                <a href="#" className="text-muted">Forgot Password?</a>
                            </div> */}
                            <button type="submit" className="btn btn-primary btn-block mt-4">Register</button>
                        </form>
                        <div className="mt-3">
                            <span>Already have an account? </span>
                            <a href="/login" className="text-primary">Sign-in</a>
                        </div>
                    </div>
                    <div className="login-image-container">
                        <img src={loginImage} alt="Login Illustration" className="login-image" />
                    </div>
                </div>
            
        </div>
    );
};

export default Register;
