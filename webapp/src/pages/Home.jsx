import React from 'react';
import Navbar from '../components/Navbar';
import { navKeys } from '../utils/navkeys';
import Layout from '../components/Layout';
import AddForm from '../components/products/AddForm';
import TableComponent from '../components/products/ProductTable';
import AdminTable from '../components/users/admin/AdminTable';


export default function Home() {
  const [selectedKey, setSelectedKey] = React.useState(navKeys[0].key); // Main page
  const [selectedSubKey, setSelectedSubKey] = React.useState(null); // Subpage (optional)

  // Handle when a main key is selected
  const handleSelectedKey = (key) => {
    setSelectedKey(key);
    setSelectedSubKey(null); // Reset subpage when selecting a new main page
  };

  // Handle when a subkey is selected
  const handleSelectedSubKey = (subKey) => {
    setSelectedSubKey(subKey);
  };

  // Function to render the content based on selected key/subkey
  const renderContent = () => {
    // Example: Multiple components for the "dashboard" page
    if (selectedKey === "dashboard" && !selectedSubKey) {
      return (
        <>
        <TableComponent />
        </>
      );
    }

    // Example: Multiple components for the "dashboard" > "milestones" subpage
    // if (selectedKey === "dashboard" && selectedSubKey === "milestones") {
    //   return (
    //     <>
    //        <AddForm />
         
    //       <div>Milestones Related Content</div>
    //     </>
    //   );
    // }

    // Example: Multiple components for the "components" > "fabrics" subpage
    if (selectedKey === "components" && selectedSubKey === "fabrics") {
      return (
        <>
          <div>Fabrics Content</div>
        </>
      );
    }
    if (selectedKey === "users" && selectedSubKey === "admins") {
        return (
          <>
           <AdminTable/>
          </>
        );
      }
    if (selectedKey === "vendors" && selectedSubKey === "addvenders") {
        return (
          <>
           
          </>
        );
      }
    // Example: Multiple components for the "libraries" > "sizes" subpage
    if (selectedKey === "libraries" && selectedSubKey === "sizes") {
      return (
        <>
          <div>Sizes Content</div>
        </>
      );
    }

    // Default content for other pages
    return <div>{selectedKey.charAt(0).toUpperCase() + selectedKey.slice(1)} Content</div>;
  };

  return (
    <div style={{ display: "flex", width: "100vw" }}>
      {/* Navbar with selection handling */}
      <Navbar
        getSelectedKey={handleSelectedKey}
        getSelectedSubKey={handleSelectedSubKey}
      />

      {/* Main and subpage rendering */}
      <div style={{ flex: 1 }}>
        <Layout title={selectedSubKey ? selectedSubKey.charAt(0).toUpperCase() + selectedSubKey.slice(1) : selectedKey.charAt(0).toUpperCase() + selectedKey.slice(1)}>
          {renderContent()}
        </Layout>
      </div>
    </div>
  );
}
