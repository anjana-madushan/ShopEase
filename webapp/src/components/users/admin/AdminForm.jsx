import React from 'react'

export default function AdminForm() {
    return (
        <div className="container-fluid">
                <div className="login-box d-flex">
                    <div className="login-form">
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
                            <div className="mb-3">
                                <input type="password" className="form-control" placeholder="password" />
                            </div>
                            <div className="mb-3">
                                <input type="password" className="form-control" placeholder="confirm password" />
                            </div>
                            <button type="submit" className="btn btn-primary btn-block mt-4">Register</button>
                        </form>
                    </div>
                </div>
            
        </div>
    );
}
