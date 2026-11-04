
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Movie,
  CreateMovieRequest,
  UpdateMovieRequest
} from '../models/movie';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  private readonly apiUrl = 'http://localhost:5005/api/movies';

  constructor(private http: HttpClient) {
  }

  // Get movies with pagination and optional title filtering
  getMovies(
    pageNumber: number = 1,
    pageSize: number = 10,
    title?: string
  ): Observable<Movie[]> {

    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (title?.trim()) {
      params = params.set('title', title.trim());
    }

    return this.http.get<Movie[]>(this.apiUrl, { params });
  }

  // Get one movie using its ID
  getMovieById(id: number): Observable<Movie> {

    return this.http.get<Movie>(`${this.apiUrl}/${id}`);
  }

  // Search for a movie using its title
  searchMovieByTitle(title: string): Observable<Movie> {

    const params = new HttpParams()
      .set('title', title);

    return this.http.get<Movie>(
      `${this.apiUrl}/search`,
      { params }
    );
  }

  // Add a new movie
  createMovie(request: CreateMovieRequest): Observable<Movie> {

    return this.http.post<Movie>(this.apiUrl, request);
  }

  // Update an existing movie
  updateMovie(
    id: number,
    request: UpdateMovieRequest
  ): Observable<void> {

    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  // Delete a movie
  deleteMovie(id: number): Observable<void> {

    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
