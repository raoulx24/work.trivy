import {Component} from '@angular/core';
import {ColDef} from "ag-grid-community";
import {WeatherForecastsService} from "../../api/services/weather-forecasts.service";
import {WeatherForecast} from "../../api/models/weather-forecast";

@Component({
  selector: 'app-fetch-weather-forecasts-data',
  templateUrl: './fetch-weather-forecasts-data.component.html',
})
export class FetchWeatherForecastsDataComponent {
  public forecasts: WeatherForecast[] = [];
  public columnDefs: ColDef[] = [
    {headerName: 'Date', field: "date", filter: true, flex: 1},
    {headerName: 'Temp. (C)', field: "temperatureC", filter: true, flex: 1},
    {headerName: 'Temp. (F)', field: "temperatureF", filter: true, flex: 1},
    {headerName: 'Summary', field: "summary", filter: true, flex: 1}
  ];

  constructor(weatherForecastsService: WeatherForecastsService) {
    weatherForecastsService.get$Json().subscribe(result => this.forecasts = result, error => console.error(error))
  }
}
