import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import 'fetch';

@inject(HttpClient)
export class TagSteps {
  heading = 'Describe Yourself';
  tags = [];

  constructor(http) {
    http.configure(config => {
      config
        .useStandardConfiguration()
        .withBaseUrl('http://localhost:55753/api/');
    });

    this.http = http;
  }

  activate() {
    return this.http.fetch('tags/demographic')
      .then(response => response.json())
      .then(tags => this.tags = tags);
  }
}
