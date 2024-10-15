class Port {
  name: string;
  constructor(name: string) {
    this.name = name;
  }
  public async connect() {
    return await chrome.runtime.connectNative(this.name);
  }
}

export default Port;
