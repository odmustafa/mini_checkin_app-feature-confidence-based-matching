import csv
import os
import sys
import shutil
from datetime import datetime

def create_sorted_copy(csv_path, output_path=None):
    """
    Create a sorted copy of the Scan-ID CSV file by the CREATED column (first column),
    placing the newest entries at the top.
    
    Args:
        csv_path: Path to the original Scan-ID CSV file
        output_path: Path for the sorted copy (if None, uses csv_path + '_sorted.csv')
    
    Returns:
        Path to the sorted copy, or None if an error occurred
    """
    try:
        # Check if original file exists
        if not os.path.exists(csv_path):
            print(f"Error: CSV file not found at {csv_path}")
            return None
        
        # Determine output path if not provided
        if output_path is None:
            file_dir = os.path.dirname(csv_path)
            file_name = os.path.basename(csv_path)
            name, ext = os.path.splitext(file_name)
            output_path = os.path.join(file_dir, f"{name}_sorted{ext}")
        
        # Make a copy of the original file
        shutil.copy2(csv_path, output_path)
        
        # Read the CSV file
        with open(output_path, 'r', newline='') as csvfile:
            reader = csv.reader(csvfile)
            header = next(reader)  # Get the header row
            rows = list(reader)    # Get all data rows
        
        # Parse dates and sort rows (newest first)
        # The date format is: "2025/05/03 23:39:46 (Sat May 03)"
        sorted_rows = []
        for row in rows:
            try:
                if row and row[0] and not row[0].startswith('npm '):
                    # Only include rows with valid date format
                    date_part = row[0].split(' (')[0]
                    datetime.strptime(date_part, '%Y/%m/%d %H:%M:%S')
                    sorted_rows.append(row)
            except Exception as e:
                print(f"Skipping invalid row: {row}")
        
        # Sort the valid rows
        sorted_rows = sorted(
            sorted_rows, 
            key=lambda row: datetime.strptime(row[0].split(' (')[0], '%Y/%m/%d %H:%M:%S'),
            reverse=True  # Newest first
        )
        
        # Write the sorted data to the copy
        with open(output_path, 'w', newline='') as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(header)  # Write the header row
            writer.writerows(sorted_rows)  # Write the sorted data rows
            
        print(f"Successfully created sorted copy with {len(sorted_rows)} records at {output_path}")
        return output_path
        
    except Exception as e:
        print(f"Error creating sorted copy: {str(e)}")
        return None

# If run directly, use the first argument as the CSV path
if __name__ == "__main__":
    if len(sys.argv) > 1:
        original_csv_path = sys.argv[1]
        output_path = sys.argv[2] if len(sys.argv) > 2 else None
        create_sorted_copy(original_csv_path, output_path)
    else:
        print("Please provide the path to the Scan-ID CSV file as an argument")
