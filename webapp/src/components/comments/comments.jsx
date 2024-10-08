import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { getCommentsBasedOnVendor } from "../../api/services/commentService";

export default function Comments() {
  const [commentData, setCommentData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Get the token from the Redux store
  const token = useSelector((state) => state.auth.loggedUser.token);

  useEffect(() => {
    try {
      if (token) {
        getCommentsBasedOnVendor(token)
          .then((response) => {
            console.log("API Response:", response.data); // Log the API response
            setCommentData(response); // Store the array of admin objects in state
            setLoading(false);
            console.log("comments Data:", commentData);
          })
          .catch((error) => {
            console.error(
              "Error fetching vendor profiles:",
              error.response || error
            ); // Log the error
            setError("Failed to fetch vendor profiles");
            setLoading(false);
          });
      } else {
        console.error("No token found. Please log in.");
        setError("No token found. Please log in.");
        setLoading(false);
      }
    } catch (error) {
      alert("Error fetching admin profiles");
      console.error("Error fetching admin profiles:", error);
      setError("Failed to fetch admin profiles");
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
      <div style={{ paddingBottom: "20px" }}>
        <h2></h2>
      </div>

      <div className="table-wrapper" >
        <table className="table custom-table">
          <thead>
            <tr>
              <th scope="col">Comment</th>
              <th scope="col">Rate</th>
            </tr>
          </thead>
          <tbody>
            {commentData?.length > 0 ? (
              commentData.map((comment) => (
                <tr key={comment.id}>
                  <td>{comment.comment}</td>
                  <td>{comment.rating}/10</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="3">There are not comments or ratings</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
