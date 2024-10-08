import React, { useEffect, useState } from 'react';
import { getAdminProfiles } from '../../../api/userApi'; 
import { useSelector } from 'react-redux'; 

export default function AdminTable() {
  const [adminData, setAdminData] = useState([]); 
  const [loading, setLoading] = useState(true); 
  const [error, setError] = useState(null); 

  // Get the token from the Redux store
    const token = useSelector((state) => state.user.token);
    console.log('Token:', token); // Log the token to ensure it is retrieved
    useEffect(() => {
    
    if (token) {
      //console.log('Token:', token); // Log the token to ensure it is retrieved
      getAdminProfiles(token)
        .then((response) => {
          console.log('API Response:', response.data); // Log the API response
          setAdminData(response.data); // Store the array of admin objects in state
          setLoading(false); // Set loading to false once data is fetched
        })
        .catch((error) => {
          console.error('Error fetching admin profiles:', error.response || error); // Log the error
          setError('Failed to fetch admin profiles');
          setLoading(false);
        });
    } else {
      console.error('No token found. Please log in.');
      setError('No token found. Please log in.');
      setLoading(false);
    }
  }, [token]);

  // Render loading message while data is being fetched
  if (loading) {
    return <p>Loading...</p>;
  }

  // Render error message if fetching failed
  if (error) {
    return <p>{error}</p>;
  }

  // Render the table with admin profiles
  return (
    <div className="container">
      <div className="table-wrapper">
        <table className="table custom-table">
          <thead>
            <tr>
              <th scope="col">Id</th>
              <th scope="col">Username</th>
              <th scope="col">Email</th>  
            </tr>
          </thead>
          <tbody>
            {adminData.length > 0 ? (
              adminData.map((admin) => (
                <tr key={admin.id}>
                  <td>{admin.id}</td>
                  <td>{admin.username}</td>
                  <td>{admin.email}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">No admin profiles found</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
