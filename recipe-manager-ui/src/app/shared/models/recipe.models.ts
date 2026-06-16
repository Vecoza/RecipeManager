export interface TagDto {
  id: string;
  name: string;
}

export interface IngredientDto {
  id: string;
  name: string;
  quantity: number;
  unit: string;
  sortOrder: number;
}

export interface StepDto {
  id: string;
  instruction: string;
  stepNumber: number;
}

export interface RecipeListItem {
  id: string;
  title: string;
  description?: string;
  imageUrl?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  createdAt: string;
  tags: TagDto[];
}

export interface RecipeDetail {
  id: string;
  userId: string;
  title: string;
  description?: string;
  imageUrl?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  createdAt: string;
  updatedAt: string;
  ingredients: IngredientDto[];
  steps: StepDto[];
  tags: TagDto[];
}

export interface CreateIngredientDto {
  name: string;
  quantity: number;
  unit: string;
}

export interface CreateStepDto {
  instruction: string;
}

export interface CreateRecipeDto {
  title: string;
  description?: string;
  imageUrl?: string;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings: number;
  ingredients: CreateIngredientDto[];
  steps: CreateStepDto[];
  tagIds: string[];
}
