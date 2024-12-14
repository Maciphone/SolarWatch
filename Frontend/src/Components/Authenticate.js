const Authenticate = async (url, options = {}) => {


  const getToken = () => {
    const name = "token=";
    const decodedCookie = decodeURIComponent(document.cookie);

    const ca = decodedCookie.split(";");
    for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) === " ") {
        c = c.substring(1);
      }
      if (c.indexOf(name) === 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  };

  const token = getToken();
  const headers = {
    "Content-Type": "application/json",
    ...options.headers,
  };
  console.log(token);
  if (token) {
    headers["Authorization"] = "Bearer " + token;
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });
  if (response.ok) {
    console.log("van response")
    console.log(response.status)

  }

  if (!response.ok) {
    throw new Error("Failed to fetch");
  }

  const data = await response.json();
  console.log("van data de hol")
  console.log(data);


  return data;
};

export default Authenticate;
