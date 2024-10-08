import React from 'react';
import Navbar from '../components/Navbar';
import { navKeys } from '../utils/navkeys';
import Layout from '../components/Layout';
import AdminTable from '../components/users/admin/AdminTable';
import CsrList from '../components/users/csr/CsrList';
import VenderList from '../components/users/vendor/venderList';
import Products from '../components/products/products';
import Comments from '../components/comments/comments';
import CustomerList from '../components/customer/CustomerList';

export default function Home() {
  const [selectedKey, setSelectedKey] = React.useState(navKeys[0].key);
  const [selectedSubKey, setSelectedSubKey] = React.useState(null);

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
    if (selectedKey === "dashboard" && !selectedSubKey) {
      return (
        <>
          <Products />
        </>
      );
    }
    if (selectedKey === "users" && selectedSubKey === "admins") {
      return (
        <>
          <AdminTable />
        </>
      );
    }
    if (selectedKey === "users" && selectedSubKey === "csr") {
      return (
        <>
          <CsrList />
        </>
      );
    }
    if (selectedKey === "orders" && !selectedSubKey) {
      return (
        <>

        </>
      );
    }
    if (selectedKey === "products" && !selectedSubKey) {
      return (
        <>

        </>
      );
    }

    if (selectedKey === "venders" && !selectedSubKey) {
      return (
        <>
          <VenderList />
        </>
      );
    }
    if (selectedKey === "inventory" && !selectedSubKey) {
      return (
        <>

        </>
      );

    }

    if (selectedKey === "comments" && !selectedSubKey) {
      return (
        <>
          <Comments />
        </>
      );

    }
        

      if (selectedKey === "customers" && !selectedSubKey) {
        return (
          <>
            <CustomerList/>
          </>
        );
      }
     
    if (selectedKey === "venders" && !selectedSubKey) {
        return (
          <>
           <VenderList/>
          </>
        );
      }
      if (selectedKey === "inventory" && !selectedSubKey) {
        return (
          <>

          </>
        );
      }


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
