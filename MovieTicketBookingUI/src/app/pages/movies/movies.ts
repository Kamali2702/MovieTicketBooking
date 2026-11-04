
import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';

import { Movie } from '../../models/movie';
import { MovieService } from '../../services/movie.service';

@Component({
  selector: 'app-movies',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './movies.html',
  styleUrl: './movies.css'
})
export class Movies implements OnInit {

  movies: Movie[] = [];
  isLoading = false;
  errorMessage = '';

  searchTerm = '';
  currentPage = 1;
  pageSize = 10;
  hasNextPage = false;

  private posterUrls: Record<string, string> = {
    'kaththi':
      'https://m.media-amazon.com/images/M/MV5BMDkwMDdmYmYtM2VlOS00NjM1LWFiZTItOTJlOWU0ZjZiMjA0XkEyXkFqcGc%40._V1_FMjpg_UX1000_.jpg',

    'leo':
      'https://media.senscritique.com/media/000021439997/0/leo.png'
  };

  constructor(
    private movieService: MovieService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadMovies();
  }

  getPosterUrl(title: string): string | null {
    return this.posterUrls[title.trim().toLowerCase()] ?? null;
  }

  loadMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.markForCheck();

    this.movieService.getMovies(
      this.currentPage,
      this.pageSize + 1,
      this.searchTerm
    ).subscribe({
      next: (data) => {
        this.hasNextPage = data.length > this.pageSize;
        this.movies = data.slice(0, this.pageSize);
        this.isLoading = false;

        this.cdr.markForCheck();
      },
      error: (error) => {
        this.errorMessage = 'Unable to load movies. Please try again.';
        this.isLoading = false;

        console.error('Error loading movies:', error);
        this.cdr.markForCheck();
      }
    });
  }

  searchMovies(title: string): void {
    this.searchTerm = title.trim();
    this.currentPage = 1;
    this.loadMovies();
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.currentPage = 1;
    this.loadMovies();
  }

  nextPage(): void {
    if (this.hasNextPage && !this.isLoading) {
      this.currentPage++;
      this.loadMovies();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1 && !this.isLoading) {
      this.currentPage--;
      this.loadMovies();
    }
  }
}
