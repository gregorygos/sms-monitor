import React, {useState} from 'react';
import AccountSummary from './components/AccountSummary';
import PhoneNumberSummary from './components/PhoneNumberSummary';
import CheckCanSend from './components/CheckCanSend';
import logo from './assets/logo.png';
import './App.css';

function App() {
  const [useDummyData, setUsedDummyData] = useState(false);

  function handleDummyButton () {
    // I know this is bad form.
    setUsedDummyData(!useDummyData);
  }

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" padding='1000px' width='166px' alt="logo" />
        <h1>SMS Mango Meter</h1>
      </header>
      <CheckCanSend />
      <AccountSummary useDummyData={useDummyData}/> 
      <PhoneNumberSummary userDummyData={useDummyData}/>
      <button onClick={handleDummyButton}> Use Dummy Data </button>
    </div>
  );
}

export default App;
